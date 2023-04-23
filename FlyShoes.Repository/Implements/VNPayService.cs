using FlyShoes.Common.Models;
using FlyShoes.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlyShoes.DAL.Implements
{
    public class VNPayService : IVNPayService
    {
        IConfigurationSection _vnpConfig;

        public VNPayService(IConfiguration configuration)
        {
            _vnpConfig = configuration.GetSection("VNPay");
        }

        public async Task<string> GetURLRedirect(OrderShoes orderShoes)
        {
            string vnp_Returnurl = _vnpConfig["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = _vnpConfig["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _vnpConfig["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = _vnpConfig["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VNPayLibrary vnpay = new VNPayLibrary();

            vnpay.AddRequestData("vnp_Version", VNPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (orderShoes.TotalBill * 100).ToString());
            vnpay.AddRequestData("vnp_BankCode", orderShoes.BankCode);

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang: " + orderShoes.OrderID);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", orderShoes.OrderID.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

        public async Task Refund(VNPayLibrary vnpayLibrary)
        {
            var vnp_Api = _vnpConfig["vnp_Api"];
            var vnp_HashSecret = _vnpConfig["vnp_HashSecret"]; //Secret KEy
            var vnp_TmnCode = _vnpConfig["vnp_TmnCode"]; // Terminal Id

            var vnp_RequestId = DateTime.Now.Ticks.ToString(); //Mã hệ thống merchant tự sinh ứng với mỗi yêu cầu hoàn tiền giao dịch. Mã này là duy nhất dùng để phân biệt các yêu cầu truy vấn giao dịch. Không được trùng lặp trong ngày.
            var vnp_Version = VNPayLibrary.VERSION; //2.1.0
            var vnp_Command = "refund";
            var vnp_TransactionType = "02";
            var vnp_Amount = Convert.ToInt64(vnpayLibrary.GetResponseData("vnp_Amount"));
            var vnp_TxnRef = vnpayLibrary.GetResponseData("vnp_TxnRef"); // Mã giao dịch thanh toán tham chiếu
            var vnp_OrderInfo = "Hoan tien giao dich:" + vnpayLibrary.GetResponseData("vnp_TxnRef");
            var vnp_TransactionNo = vnpayLibrary.GetResponseData("vnp_TransactionNo"); //Giả sử giá trị của vnp_TransactionNo không được ghi nhận tại hệ thống của merchant.
            var vnp_TransactionDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnp_IpAddr = Utils.GetIpAddress();

            var signData = vnp_RequestId + "|" + vnp_Version + "|" + vnp_Command + "|" + vnp_TmnCode + "|" + vnp_TransactionType + "|" + vnp_TxnRef + "|" + vnp_Amount + "|" + vnp_TransactionNo + "|" + vnp_TransactionDate + "|" + "mahhu" + "|" + vnp_CreateDate + "|" + vnp_IpAddr + "|" + vnp_OrderInfo;
            var vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);

            var rfData = new
            {
                vnp_RequestId = vnp_RequestId,
                vnp_ResponseId = DateTime.Now.Ticks.ToString(),
                vnp_ResponseCode = "00",
                vnp_Message = "Refund thành công",
                vnp_BankCode = vnpayLibrary.GetResponseData("vnp_BankCode"),
                vnp_Version = vnp_Version,
                vnp_Command = vnp_Command,
                vnp_TmnCode = vnp_TmnCode,
                vnp_TransactionType = vnp_TransactionType,
                vnp_TxnRef = vnp_TxnRef,
                vnp_Amount = vnp_Amount,
                vnp_OrderInfo = vnp_OrderInfo,
                vnp_TransactionNo = vnp_TransactionNo,
                vnp_TransactionDate = vnp_TransactionDate,
                vnp_CreateBy = "mahhu",
                vnp_CreateDate = vnp_CreateDate,
                vnp_IpAddr = vnp_IpAddr,
                vnp_TransactionStatus = "00",
                vnp_SecureHash = vnp_SecureHash
            };
            var jsonData = JsonSerializer.Serialize(rfData);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(vnp_Api);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonData);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var strData = "";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                strData = streamReader.ReadToEnd();
            }
        }
    }
}
