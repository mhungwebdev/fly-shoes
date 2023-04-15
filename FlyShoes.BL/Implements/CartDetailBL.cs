using FlyShoes.BL.Implements;
using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using FlyShoes.DAL.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.BL.Base
{
    public class CartDetailBL : BaseBL<CartDetail>,ICartDetailBL
    {
        public CartDetailBL(IDatabaseService databaseService):base(databaseService)
        {

        }

        public async Task<List<CartDetail>> GetCartDetailByUser(int userID)
        {
            var param = new Dictionary<string, object>()
            {
                {"v_UserID",userID }
            };

            var cartDetails = await _dataBaseService.QueryUsingStoredProcedureAsync<CartDetail>("Proc_CartDetail_GetByUser",param);
            var idVouchers = cartDetails.Where(cd => cd.VoucherID != null)?.Select(cd => cd.VoucherID).ToList();

            if (cartDetails != null && cartDetails.Count > 0 && idVouchers != null && idVouchers.Count > 0)
            {
                var commandGetVoucher = "SELECT * FROM Voucher v WHERE v.VoucherID IN ({0}) AND v.VoucherID NOT IN(SELECT vu.VoucherID FROM VoucherUsed vu WHERE vu.VoucherID IN ({1}) AND vu.UserID = @UserID) AND v.IsActive IS TRUE AND v.EndDate > @NOW AND v.Quantity > 0;";
                commandGetVoucher = string.Format(commandGetVoucher, string.Join(",", idVouchers), string.Join(",", idVouchers));
                var paramGetVoucher = new Dictionary<string, object>() {
                    { "@UserID", userID },
                    {"@NOW",DateTime.Now }
                };
                var vouchers = await _dataBaseService.QueryUsingCommanTextAsync<Voucher>(commandGetVoucher, paramGetVoucher);

                if (vouchers != null && vouchers.Count > 0)
                {
                    foreach (var cd in cartDetails)
                    {
                        var voucher = vouchers.Find(voucher => voucher.VoucherID == cd.VoucherID);
                        cd.Voucher = voucher;
                    }
                }
            }

            return cartDetails;
        }

        public override List<ValidateResult> ValidateBeforeSave(CartDetail entity)
        {
            var validateInfo = base.ValidateBeforeSave(entity);

            var commandCheckShoesExistInCart = "SELECT COUNT(1) FROM CartDetail WHERE ShoesID = @ShoesID AND UserID = @UserID";
            var paramCheck = new Dictionary<string, object>() {
                {"@ShoesID",entity.ShoesID },
                {"@UserID",entity.UserID }
            };
            var res = _dataBaseService.ExecuteScalarUsingCommandText<int>(commandCheckShoesExistInCart, paramCheck) > 0;

            if (res)
            {
                validateInfo.Add(new ValidateResult()
                {
                    ErrorCode = Common.Enums.ErrorCodeEnum.Exist
                });
            }

            return validateInfo;
        }
    }
}
