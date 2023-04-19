using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Constants;
using FlyShoes.Common.Enums;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FlyShoes.API.Controllers
{
    public class OrderShoesController : FlyShoesController<OrderShoes>
    {
        IOrderShoesBL _orderShoesBL;
        public OrderShoesController(IOrderShoesBL orderBL):base(orderBL)
        {
            _orderShoesBL= orderBL;
        }

        [HttpPost("order/{paymentType}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleTypeConstant.CUSTOMER)]
        public async Task<ServiceResponse> Order(int paymentType,OrderShoes orderShoes)
        {
            var result = await _orderShoesBL.Order(paymentType, orderShoes);

            return result;
        }

        [HttpGet("order-by-user")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleTypeConstant.CUSTOMER)]
        public async Task<ServiceResponse> GetOrderByUser()
        {
            var result = new ServiceResponse();

            result.Data = await _orderShoesBL.GetOrdersByUser();

            return result;
        }

        [HttpPost("update-order-status")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleTypeConstant.ADMIN)]
        public async Task<ServiceResponse> UpdateOrderStatus([FromBody]Dictionary<string,object> orderInfor)
        {
            var result = new ServiceResponse();

            var orderID = orderInfor.GetValue<int>("OrderShoesID");
            var orderStatus = orderInfor.GetValue<OrderStatus>("OrderStatus");
            result.Success = await _orderShoesBL.UpdateOrderStatus(orderID,orderStatus) > 0;

            return result;
        }
    }
}
