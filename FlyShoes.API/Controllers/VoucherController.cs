using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class VoucherController : FlyShoesController<Voucher>
    {
        public VoucherController(IVoucherBL voucherBL):base(voucherBL)
        {

        }
    }
}
