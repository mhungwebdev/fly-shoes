using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class OrderShoesController : FlyShoesController<OrderShoes>
    {
        public OrderShoesController(IOrderShoesBL orderBL):base(orderBL)
        {

        }
    }
}
