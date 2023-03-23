using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class CartDetailController : FlyShoesController<CartDetail>
    {
        public CartDetailController(ICartDetailBL cartDetailBL):base(cartDetailBL)
        {

        }
    }
}
