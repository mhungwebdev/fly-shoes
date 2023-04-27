using Amazon.Runtime.Internal.Transform;
using Firebase.Auth;
using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Constants;
using FlyShoes.Common.Enums;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using FlyShoes.DAL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Net;
using System.Text.Json;
using static Google.Cloud.Firestore.V1.StructuredAggregationQuery.Types.Aggregation.Types;
using static Google.Rpc.Context.AttributeContext.Types;

namespace FlyShoes.API.Controllers
{
    public class OrderShoesController : FlyShoesController<OrderShoes>
    {
        IOrderShoesBL _orderShoesBL;
        IDatabaseService _databaseBL;
        IConfigurationSection _vnpConfig;
        IVNPayService _vnpayService;
        IPaymentInfoBL _paymentInfoBL;
        IFirestoreService _firestoreService;

        public OrderShoesController(IOrderShoesBL orderBL,IFirestoreService firestoreService, IDatabaseService databaseService, IConfiguration configuration,IPaymentInfoBL paymentInfoBL,IVNPayService vNPayService) :base(orderBL)
        {
            _orderShoesBL= orderBL;
            _databaseBL= databaseService;
            _vnpConfig = configuration.GetSection("VNPay");
            _vnpayService = vNPayService;
            _paymentInfoBL = paymentInfoBL;
            _firestoreService = firestoreService;
        }

        [HttpPost("order/{paymentType}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleTypeConstant.CUSTOMER)]
        public async Task<ServiceResponse> Order(int paymentType,OrderShoes orderShoes)
        {
            var result = await _orderShoesBL.Order(paymentType, orderShoes);

            return result;
        }

        [HttpGet("order-by-user")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleTypeConstant.CUSTOMER)]
        public async Task<ServiceResponse> GetOrderByUser()
        {
            var result = new ServiceResponse();

            result.Data = await _orderShoesBL.GetOrdersByUser();

            return result;
        }

        [HttpPost("update-order-status")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleTypeConstant.ADMIN)]
        public async Task<ServiceResponse> UpdateOrderStatus([FromBody]Dictionary<string,object> orderInfor)
        {
            var result = new ServiceResponse();

            var orderID = orderInfor.GetValue<int>("OrderShoesID");
            var orderStatus = orderInfor.GetValue<OrderStatus>("OrderStatus");
            result.Success = await _orderShoesBL.UpdateOrderStatus(orderID,orderStatus) > 0;

            return result;
        }

        [HttpGet("PaymentInfo")]    
        public async Task<IActionResult> HookVNPay()
        {

            if (Request.Query.Count > 0)
            {
                string vnp_HashSecret = _vnpConfig["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.Query.ToList();
                VNPayLibrary vnpay = new VNPayLibrary();

                foreach (var query in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(query.Value) && query.Key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(query.Key, query.Value);
                    }
                }

                var vnp_SecureHash = vnpay.GetResponseData("vnp_SecureHash");
                var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                var vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var orderID = vnpay.GetResponseData("vnp_TxnRef");
                        var commandUpdate = "UPDATE OrderShoes SET PaymentMethod = @PaymentMethod,PaymentStatus = TRUE WHERE OrderID = @OrderID";
                        var paramUpdate = new Dictionary<string, object>()
                        {
                            {"@PaymentMethod",PaymentMethod.VNPay },
                            {"@OrderID",orderID }
                        };

                        var success = _databaseBL.ExecuteUsingCommandText(commandUpdate, paramUpdate) > 0;

                        if (!success)
                        {
                            await _vnpayService.Refund(vnpay);
                            return StatusCode(500, "Có lỗi xảy ra ! Bạn đã được hoàn lại tiền");
                        }

                        var paymentInfo = new PaymentInfo()
                        {
                            OrderID = int.Parse(orderID),
                            Amount = vnpay.GetResponseData("vnp_Amount"),
                            BankCode = vnpay.GetResponseData("vnp_BankCode"),
                            State = ModelStateEnum.Insert,
                            TransactionNo = vnpay.GetResponseData("vnp_TransactionNo"),
                            CreatedDate = DateTime.Now
                        };

                        _ = _paymentInfoBL.Save(paymentInfo);

                        var commandGetUserID = "SELECT UserID FROM OrderShoes WHERE OrderID = @OrderID";

                        var userID = _databaseBL.ExecuteScalarUsingCommandText<int>(commandGetUserID, new Dictionary<string, object> { { "@OrderID", orderID } });

                        await _firestoreService.PushNotification(new Notification()
                        {
                            UserID = userID,
                            Message = "Đơn hàng của bạn đã được thanh toán thành công ❤️"
                        });

                        return Ok("Thanh toán thành công ! Cảm ơn quý khách");
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        return StatusCode(400, "Thanh toán không thành công");
                    }
                }
                else
                {
                    return StatusCode(400, "Có lỗi trong quá trình xử lý");
                }
            }

            return StatusCode(500, "Có lỗi xảy ra");
        }
    }
}
