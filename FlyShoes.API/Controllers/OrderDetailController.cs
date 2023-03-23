using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class OrderDetailController : FlyShoesController<OrderDetail>
    {
        public OrderDetailController(IOrderDetailBL orderDetailBL):base(orderDetailBL)
        {

        }
    }
}
