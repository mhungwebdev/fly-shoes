using FlyShoes.Common.Enums;
using FlyShoes.Common.Models;
using FlyShoes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.BL.Interfaces
{
    public interface IOrderShoesBL : IBaseBL<OrderShoes>
    {
        public Task<ServiceResponse> Order(int paymentType, OrderShoes orderShoes);

        public Task<List<OrderShoes>> GetOrdersByUser();

        public Task<int> UpdateOrderStatus(int orderShoesID,OrderStatus orderStatus);
    }
}
