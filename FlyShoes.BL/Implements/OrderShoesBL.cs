using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Enums;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using FlyShoes.DAL.Interfaces;
using Google.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.BL.Base
{
    public class OrderShoesBL : BaseBL<OrderShoes>,IOrderShoesBL
    {
        IFirestoreService _firstoreService;
        IEmailService _emailService;
        IPaymentInfoBL _paymentInfoBL;
        IVNPayService _vnpayService;

        public OrderShoesBL(IVNPayService vnpayService, IPaymentInfoBL paymentInfoBL,IDatabaseService databaseService,IFirestoreService firestoreService, IEmailService emailService) : base(databaseService)
        {
            _firstoreService = firestoreService;
            _emailService = emailService;
            _paymentInfoBL= paymentInfoBL;
            _vnpayService = vnpayService;
        }

        public async Task<List<OrderShoes>> GetOrdersByUser()
        {
            var commandGetOrder = "Select * from OrderShoes os WHERE os.UserID = @UserID;";
            var param = new Dictionary<string, object>()
            {
                {"@UserID",_dataBaseService.CurrentUser.UserID }
            };

            return await _dataBaseService.QueryUsingCommanTextAsync<OrderShoes>(commandGetOrder,param);
        }

        public async Task<ServiceResponse> Order(int paymentType, OrderShoes orderShoes)
        {
            var result = new ServiceResponse();

            /// Thực hiện cập nhật số lượng hàng trước khi tạo đơn
            var index = 0;
            var param = new Dictionary<string, object>();
            StringBuilder updateQuantity = new StringBuilder();
            var connection = _dataBaseService.GetDbConnection();
            connection.Open();
            var transaction = connection.BeginTransaction();
            foreach(var orderDetail in orderShoes.OrderDetails)
            {
                param.Add($"@Quantity_{index}", orderDetail.Quantity);
                param.Add($"@ShoesDetailID_{index}", orderDetail.ShoesDetailID);
                updateQuantity.Append($"UPDATE ShoesDetail sd SET sd.Quantity = sd.Quantity - @Quantity_{index} WHERE sd.ShoesDetailID = @ShoesDetailID_{index} AND (sd.Quantity - @Quantity_{index} > 0 OR sd.Quantity - @Quantity_{index} = 0);");
                index++;
            }

            var resUpdateQuantity = await _dataBaseService.ExecuteUsingCommandTextAsync(updateQuantity.ToString(), param,transaction,connection);

            if(resUpdateQuantity != orderShoes.OrderDetails.Count)
            {
                transaction.Rollback();
                transaction.Dispose();
                connection.Close();
                result.Success = false;
                result.ValidateInfo.Add(new ValidateResult()
                {
                    ErrorCode = ErrorCodeEnum.ProductNotEnough
                });

                return result;
            }

            /// Nếu có voucher thì tạo voucherused
            /// Giảm số lượng voucher
            var voucherIDs = orderShoes.OrderDetails.Where(orderDetail => orderDetail.VoucherID != null)?.Select(orderDetail => orderDetail.VoucherID)?.ToList();
            if(voucherIDs != null && voucherIDs.Count > 0)
            {
                var paramUpdateVoucher = new Dictionary<string, object>()
                {
                    {"@DateNow",System.DateTime.Now }
                };
                var updateVoucher = "UPDATE Voucher v SET v.Quantity = v.Quantity - 1 WHERE v.VoucherID IN ({0}) AND (v.Quantity - 1 > 0 OR v.Quantity - 1 = 0) AND v.EndDate > @DateNow AND v.IsActive IS TRUE;";

                var resUpdateVoucher = await _dataBaseService.ExecuteUsingCommandTextAsync(string.Format(updateVoucher, string.Join(",", voucherIDs)), paramUpdateVoucher, transaction,connection);
                if(resUpdateVoucher != voucherIDs.Count)
                {
                    transaction.Rollback();
                    transaction.Dispose();
                    connection.Close();
                    result.Success = false;
                    result.ValidateInfo.Add(new ValidateResult()
                    {
                        ErrorCode = ErrorCodeEnum.VoucherNotEnough
                    });

                    return result;
                }

                StringBuilder insertVoucherUsed = new StringBuilder();
                var indexInsert = 0;
                var paramInsert = new Dictionary<string, object>()
                {
                    {"@DateNow",System.DateTime.Now },
                    {"@UserID",_dataBaseService.CurrentUser.UserID },
                    {"@UserName",_dataBaseService.CurrentUser.FullName },
                };
                foreach(var idVoucher in voucherIDs)
                {
                    insertVoucherUsed.Append($"INSERT INTO VoucherUsed (VoucherID,UserID,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy) VALUES (@VoucherID_{indexInsert},@UserID,@DateNow,@DateNow,@UserName,@UserName);");
                    paramInsert.Add($"@VoucherID_{indexInsert}", idVoucher);
                    indexInsert++;
                }
                _dataBaseService.ExecuteUsingCommandText(insertVoucherUsed.ToString(), paramInsert, transaction,connection);
            }

            /// Cập nhật số tiền bỏ ra cho user
            var commandForUser = "UPDATE User u SET u.AmountSpent = u.AmountSpent + @TotalBill WHERE u.UserID = @UserID;";
            var paramUpdateUser = new Dictionary<string, object>()
            {
                {"@TotalBill",orderShoes.TotalBill },
                {"@UserID",_dataBaseService.CurrentUser.UserID }
            };
            await _dataBaseService.ExecuteUsingCommandTextAsync(commandForUser, paramUpdateUser,transaction,connection);

            BeforeSave(orderShoes);

            /// Lưu Order   
            orderShoes.UserID = _dataBaseService.CurrentUser.UserID;
            var id = DoInsert(orderShoes,connection,transaction);
            orderShoes.OrderDetails.ForEach(orderDetail => orderDetail.OrderID = id);
            DoInsertMulti(orderShoes.OrderDetails, connection, transaction);

            /// Nếu là sản phẩm trong giở hàng xóa cartdetail
            if(paymentType == (int)PaymentType.Full)
            {
                var deleteCartDetail = "DELETE FROM CartDetail WHERE UserID = @UserID AND ShoesID IN ({0});";
                var shoesIDs = orderShoes.OrderDetails.Select(orderDetail => orderDetail.ShoesID).ToList();
                var paramDelete = new Dictionary<string, object>()
                {
                    {"@UserID",_dataBaseService.CurrentUser.UserID }
                };

                await _dataBaseService.ExecuteUsingCommandTextAsync(string.Format(deleteCartDetail, string.Join(",", shoesIDs)), paramDelete,transaction,connection);
            }

            result.Success = true;
            result.Data = id;
            transaction.Commit();
            transaction.Dispose();
            connection.Close();

            var notification = new Notification()
            {
                UserID = _dataBaseService.CurrentUser.UserID,
                Message = "Đơn hàng của bạn đã được tạo và đang chờ duyệt, cảm ơn bạn đã ủng hộ shop ❤️",
                SortOrder = System.DateTime.Now.Ticks,
            };
            _ = _firstoreService.PushNotification(notification).ConfigureAwait(false);

            return result;
        }

        public async Task<int> UpdateOrderStatus(int orderShoesID,OrderStatus orderStatus)
        {
            var updateOrder = "UPDATE OrderShoes SET Status = @Status WHERE OrderID = @OrderShoesID";
            var param = new Dictionary<string, object>()
            {
                {"@Status",orderStatus },
                {"@OrderShoesID",orderShoesID }
            };

            var connection = _dataBaseService.GetDbConnection();
            connection.Open();
            var transaction = connection.BeginTransaction();
            var order = await GetByID(orderShoesID.ToString());
            var commandGetUser = $"SELECT * FROM User WHERE UserID = @UserID";
            var user = await _dataBaseService.QuerySingleUsingCommanTextAsync<User>(commandGetUser, new Dictionary<string, object>() { { "@UserID", order.UserID } });
            var res = await _dataBaseService.ExecuteUsingCommandTextAsync(updateOrder, param, transaction, connection);

            if(order.PaymentMethod == PaymentMethod.VNPay && order.PaymentStatus)
            {
                var resPaymentInfo = await _paymentInfoBL.GetByField("OrderID",order.OrderID.ToString());
                var paymentInfo = resPaymentInfo.FirstOrDefault();

                if(paymentInfo != null)
                {
                    var vnpay = new VNPayLibrary();
                    vnpay.AddResponseData("vnp_Amount", paymentInfo.Amount);
                    vnpay.AddResponseData("vnp_TxnRef", paymentInfo.OrderID.ToString());
                    vnpay.AddResponseData("vnp_TransactionNo", paymentInfo.TransactionNo);
                    vnpay.AddResponseData("vnp_BankCode", paymentInfo.BankCode);

                    _ = _vnpayService.Refund(vnpay);
                    _ = _paymentInfoBL.Delete(paymentInfo.PaymentInfoID.ToString());
                }
            }

            if (res > 0 && orderStatus == OrderStatus.Cancel)
            {
                /// lay order
                /// lay order detail
                /// Hoan lai so luong
                StringBuilder stringBuilder = new StringBuilder();
                StringBuilder stringUpdateVoucher = new StringBuilder();

                var indexUpdateQuantity = 0;
                var quantityVoucher = 0;
                var paramUpdateQuantity = new Dictionary<string, object>();
                var paramUpdateVoucher = new Dictionary<string, object>() {
                    {"@Now",System.DateTime.Now }
                };

                foreach (var orderDetail in order.OrderDetails)
                {
                    stringBuilder.Append($"UPDATE ShoesDetail SET Quantity = Quantity + @Quantity_{indexUpdateQuantity} WHERE ShoesDetailID = @ShoesDetailID_{indexUpdateQuantity};");
                    paramUpdateQuantity.Add($"@Quantity_{indexUpdateQuantity}", orderDetail.Quantity);
                    paramUpdateQuantity.Add($"@ShoesDetailID_{indexUpdateQuantity}", orderDetail.ShoesDetailID);
                    indexUpdateQuantity++;

                    if (orderDetail.VoucherID != null)
                    {
                        stringUpdateVoucher.Append($"UPDATE Voucher SET Quantity = Quantity + 1 WHERE VoucherID = @VoucherID_{indexUpdateQuantity} AND IsActive IS TRUE AND EndDate > @Now");
                        quantityVoucher++;
                        paramUpdateVoucher.Add($"VoucherID_{indexUpdateQuantity}", orderDetail.VoucherID);
                    }
                }
                var resUpdateQuantity = await _dataBaseService.ExecuteUsingCommandTextAsync(stringBuilder.ToString(), paramUpdateQuantity, transaction, connection);
                if (quantityVoucher > 0)
                {
                    _ = _dataBaseService.ExecuteUsingCommandTextAsync(stringUpdateVoucher.ToString(), param);
                }

                if (resUpdateQuantity != order.OrderDetails.Count)
                {
                    transaction.Rollback();
                    transaction.Dispose();
                    connection.Close();
                    return 0;
                }

                var updateUser = $"UPDATE User SET AmountSpent = AmountSpent - @TotalBill WHERE UserID = @UserID";
                var resUpdateUser = await _dataBaseService.ExecuteUsingCommandTextAsync(updateUser, new Dictionary<string, object>() { { @"TotalBill", order.TotalBill }, { "@UserID", order.UserID } }, transaction, connection);
                if (resUpdateUser == 0)
                {
                    transaction.Rollback();
                    transaction.Dispose();
                    connection.Close();
                    return 0;
                }
                /// Giam tien cho
            }

            _ = SendMailNotifyOrderStatus(orderStatus, user);

            transaction.Commit();
            transaction.Dispose();
            connection.Close();
            return res;
        }

        private async Task SendMailNotifyOrderStatus(OrderStatus orderStatus, User user)
        {
            var textStatus = "";
            switch (orderStatus)
            {
                case OrderStatus.Pending:
                    break;
                case OrderStatus.Success:
                    textStatus = "thành công";
                    break;
                case OrderStatus.Confirm:
                    textStatus = "được xác nhận";
                    break;
                case OrderStatus.Cancel:
                    textStatus = "bị hủy";
                    break;
            }

            if (textStatus.Equals("")) return;
            var emilNotification = new FlyEmail()
            {
                EmailContent = $"Xin chào <b>{user.FullName}</b><br>Chúng tôi xin thông báo đơn hàng của bạn đã {textStatus}.",
                To = user.Email,
                Subject = "Fly Shoes thông báo trạng thái đơn hàng",
            };

            await _emailService.SendMail(emilNotification);

            var notification = new Notification()
            {
                UserID = user.UserID,
                Message = $"Đơn hàng của bạn đã {textStatus} ❤️",
                SortOrder = System.DateTime.Now.Ticks
            };
            _ = _firstoreService.PushNotification(notification);
        }
    }
}
