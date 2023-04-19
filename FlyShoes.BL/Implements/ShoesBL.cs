using FlyShoes.BL.Interfaces;
using FlyShoes.Common;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using FlyShoes.DAL.Implements;
using FlyShoes.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlyShoes.BL.Implements
{
    public class ShoesBL : BaseBL<Shoes>,IShoesBL
    {
        IEmailService _emailService;
        public ShoesBL(IDatabaseService databaseService,IEmailService emailService):base(databaseService)
        {
            _emailService= emailService;
        }

        public async Task<decimal> GetMaxPrice()
        {
            var command = "SELECT MAX(Price) FROM Shoes";
            return await _dataBaseService.ExecuteScalarUsingCommandTextAsync<decimal>(command);
        }

        public override void AfterSave(object entity, IDbConnection connection, IDbTransaction transaction)
        {
            base.AfterSave(entity, connection, transaction);
            _ = SendMailNotification();

            InsertShoesDetail(entity, connection, transaction);
        }

        private async Task SendMailNotification()
        {
            var commandGetUser = "SELECT DISTINCT u.Email FROM User u WHERE u.ReceiveEmail IS TRUE;";
            var emails = _dataBaseService.QueryUsingCommanText<string>(commandGetUser);
            if (emails != null && emails.Count > 0)
            {
                var emailSend = new FlyEmail()
                {
                    From = "mhung.haui.webdev@gmail.com",
                    EmailContent = "Chúng tôi có sản phẩm mới ! Hãy nhanh tay sở hữu",
                    Subject = "Fly Shoes - Thông báo sản phẩm mới",
                    To = emails.FirstOrDefault()
                };

                if(emails.Count > 1)
                {
                    emails.RemoveAt(0);
                    emailSend.Cc = emails;
                }

                _emailService.SendMail(emailSend);
            }
        }

        private void InsertShoesDetail(object entity, IDbConnection connection, IDbTransaction transaction)
        {
            var shoes = JsonSerializer.Deserialize<Shoes>(JsonSerializer.Serialize(entity));
            StringBuilder commandInsertDetail = new StringBuilder();
            var properties = new List<string>() { "SizeID", "ColorID", "Quantity", "ColorName", "SizeName", "ColorCode" };
            commandInsertDetail.Append("INSERT INTO ShoesDetail (ShoesID,SizeID,ColorID,Quantity,ColorName,SizeName,ColorCode) VALUES ");

            var param = new Dictionary<string, object>();
            var values = new List<string>();
            var index = 0;
            foreach (var shoesDetail in shoes.ShoesDetails)
            {
                var value = $"(@ShoesID,";
                foreach (var property in properties)
                {
                    value += $"@{property}_{index},";
                    param.Add($"@{property}_{index}", shoesDetail.GetValue(property));
                }
                value = value.Substring(0, value.Length - 1) + ")";
                values.Add(value);
                index++;
            }
            param.Add("@ShoesID", shoes.ShoesID);
            commandInsertDetail.Append(string.Join(",", values));
            try
            {
                _dataBaseService.ExecuteUsingCommandText(commandInsertDetail.ToString(), param, transaction, connection);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                transaction.Dispose();
                connection.Close();
                throw new FSException(ex.Message);
            }
        }

        public override void GetMoreInfo(object entity)
        {
            base.GetMoreInfo(entity);
            var voucherID = entity.GetValue<int>("VoucherID");

            if(voucherID != null)
            {
                var commandGetVoucher = "SELECT * FROM Voucher v WHERE v.VoucherID = @VoucherID;";
                var param = new Dictionary<string, object>()
                {
                    {"@VoucherID",voucherID }
                };
                var voucher = _dataBaseService.QuerySingleUsingCommanText<Voucher>(commandGetVoucher, param);
                entity.SetValue("Voucher", voucher);
            }
        }

        public async Task<List<Shoes>> GetShoesForPayment(List<int> shoesIDs, int userID)
        {
            var commandShoesPayments = $"SELECT * FROM Shoes WHERE ShoesID IN ({string.Join(",", shoesIDs)})";
            var shoesPayments = await _dataBaseService.QueryUsingCommanTextAsync<Shoes>(commandShoesPayments);

            if (shoesPayments != null && shoesPayments.Count > 0)
            {
                var idVouchers = shoesPayments.Where(s => s.VoucherID != null)?.Select(s => s.VoucherID).ToList();
                foreach(var shoesPayment in shoesPayments)
                {
                    var getShoesDetail = "SELECT * FROM ShoesDetail WHERE ShoesID = @ShoesID";
                    var param = new Dictionary<string, object>()
                    {
                        {"@ShoesID",shoesPayment.ShoesID }
                    };
                    shoesPayment.ShoesDetails = _dataBaseService.QueryUsingCommanText<ShoesDetail>(getShoesDetail, param);
                }

                var commandGetVoucher = "SELECT * FROM Voucher v WHERE v.VoucherID IN ({0}) AND v.VoucherID NOT IN(SELECT vu.VoucherID FROM VoucherUsed vu WHERE vu.VoucherID IN ({1}) AND vu.UserID = @UserID) AND v.IsActive IS TRUE AND v.EndDate > @NOW AND v.Quantity > 0;";
                commandGetVoucher = string.Format(commandGetVoucher, string.Join(",", idVouchers), string.Join(",", idVouchers));
                var paramGetVoucher = new Dictionary<string, object>() {
                    { "@UserID", userID },
                    {"@NOW",DateTime.Now }
                };
                var vouchers = _dataBaseService.QueryUsingCommanText<Voucher>(commandGetVoucher, paramGetVoucher);

                if (vouchers != null && vouchers.Count > 0)
                {
                    foreach (var s in shoesPayments)
                    {
                        var voucher = vouchers.Find(voucher => voucher.VoucherID == s.VoucherID);
                        s.Voucher = voucher;
                    }
                }
            }

            return shoesPayments;
        }
    }
}
