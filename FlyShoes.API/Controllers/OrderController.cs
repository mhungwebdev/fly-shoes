using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class OrderController : FlyShoesController<Order>
    {
        public OrderController(IOrderBL orderBL):base(orderBL)
        {

        }
    }
}
