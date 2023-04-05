using FlyShoes.BL.Interfaces;
using FlyShoes.Common;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
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
            SendMailNotification();

            InsertShoesDetail(entity, connection, transaction);
        }

        private void SendMailNotification()
        {
            //var commandGetUser = "SELECT DISTINCT u.Email FROM User u WHERE u.IsReceiveEmailNewShoes IS TRUE;";
            //var emails = _dataBaseService.QueryUsingCommanText<string>(commandGetUser);
            //if(emails != null && emails.Count > 0)
            //{
            //    var emailSend = new FlyEmail()
            //    {
            //        From = "mhung.haui.webdev@gmail.com",
            //        Cc = emails,
            //        EmailContent = "Chúng tôi có sản phẩm mới ! Hãy nhanh tay sở hữu",
            //        Subject = "Fly Shoes - Thông báo sản phẩm mới",
            //        To = "mahhugcoder@gmail.com"
            //    };
            //    _emailService.SendMail(emailSend);
            //}
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
    }
}
