using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class VoucherController : FlyShoesController<Voucher>
    {
        IVoucherBL _voucherBL;
        public VoucherController(IVoucherBL voucherBL):base(voucherBL)
        {
            _voucherBL= voucherBL;
        }

        [HttpGet("check-voucher")]
        public bool CheckVoucherUsage(int voucherID,int? userID)
        {
            return _voucherBL.CheckVoucherUsage(voucherID,userID);
        }
    }
}
