using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class ShoesController : FlyShoesController<Shoes>
    {
        IShoesBL _shoesBL;
        IDatabaseService _databaseService;

        public ShoesController(IShoesBL shoesBL,IDatabaseService databaseService):base(shoesBL)
        {
            _shoesBL = shoesBL;
            _databaseService = databaseService;
        }

        [HttpGet("max-price")]
        public async Task<ServiceResponse> GetMaxPrice()
        {
            var response = new ServiceResponse();

            response.Data = await _shoesBL.GetMaxPrice();

            return response;
        }

        [HttpPost("paging")]
        public override async Task<ServiceResponse> Paging(PagingPayload pagingPayload)
        {
            var result = new ServiceResponse();
            var shoes = await _shoesBL.Paging(pagingPayload);
            var idVouchers = shoes.Where(s => s.VoucherID != null)?.Select(s => s.VoucherID).ToList();

            if(shoes != null && shoes.Count > 0 && idVouchers != null && idVouchers.Count > 0)
            {
                var commandGetVoucher = "SELECT * FROM Voucher v WHERE v.VoucherID IN ({0}) AND v.VoucherID NOT IN(SELECT vu.VoucherID FROM VoucherUsed vu WHERE vu.VoucherID IN ({1}) AND vu.UserID = @UserID) AND v.IsActive IS TRUE AND v.EndDate > @NOW AND v.Quantity > 0;";
                commandGetVoucher = string.Format(commandGetVoucher, string.Join(",", idVouchers), string.Join(",", idVouchers));
                var param = new Dictionary<string, object>() {
                    { "@UserID", _databaseService.CurrentUser?.UserID },
                    {"@NOW",DateTime.Now }
                };
                var vouchers = _databaseService.QueryUsingCommanText<Voucher>(commandGetVoucher,param);

                if(vouchers != null && vouchers.Count > 0)
                {
                    foreach(var s in shoes)
                    {
                        var voucher = vouchers.Find(voucher => voucher.VoucherID == s.VoucherID);
                        s.Voucher = voucher;
                    }
                }
            }

            result.Data = shoes;
            return result;
        }
    }
}
