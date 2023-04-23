using FlyShoes.Common.Models;
using FlyShoes.DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X9;

namespace FlyShoes.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VNPayController : ControllerBase
    {
        IVNPayService _vnpayService;
        public VNPayController(IVNPayService vnpayService)
        {
            _vnpayService = vnpayService;
        }

        [HttpPost("url-redirect")]
        public async Task<string> GetURLRedirect(OrderShoes orderShoes)
        {
            string paymentUrl = await _vnpayService.GetURLRedirect(orderShoes);

            return paymentUrl;
        }
    }
}
