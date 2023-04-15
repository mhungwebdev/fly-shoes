using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class CartDetailController : FlyShoesController<CartDetail>
    {
        ICartDetailBL _cartDetailBL;
        public CartDetailController(ICartDetailBL cartDetailBL):base(cartDetailBL)
        {
            _cartDetailBL = cartDetailBL;
        }

        [HttpGet("cart-detail/{userID}")]
        public async Task<ServiceResponse> GetCartDetailByUser(int userID)
        {
            var result = new ServiceResponse();

            result.Data = await _cartDetailBL.GetCartDetailByUser(userID);

            return result;
        }
    }
}
