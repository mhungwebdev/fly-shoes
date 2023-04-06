using FlyShoes.BL.Interfaces;
using FlyShoes.Common;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlyShoes.BL.Implements
{
    public class VoucherBL : BaseBL<Voucher>,IVoucherBL
    {
        IDatabaseService _databaseService;
        public VoucherBL(IDatabaseService databaseService):base(databaseService)
        {
            _databaseService = databaseService;
        }

        public override void AfterSave(object entity, IDbConnection connection, IDbTransaction transaction)
        {
            base.AfterSave(entity, connection, transaction);

            var voucher = JsonSerializer.Deserialize<Voucher>(JsonSerializer.Serialize(entity));

            var updateShoes = "UPDATE Shoes SET VoucherID = @VoucherID WHERE ShoesID in ({0})";

            try
            {
                var param = new Dictionary<string, object>()
                {
                    {"@VoucherID", voucher.VoucherID}
                };
                var res = _dataBaseService.ExecuteUsingCommandText(string.Format(updateShoes,string.Join(",",voucher.ShoesIDApply)),param,transaction,connection) > 0;

                if (!res) {
                    transaction.Rollback();
                    transaction.Dispose();
                    connection.Close();
                };
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
