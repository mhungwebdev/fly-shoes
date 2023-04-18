using FlyShoes.Common.Models;
using FlyShoes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.BL.Interfaces
{
    public interface IShoesBL : IBaseBL<Shoes>
    {
        public Task<decimal> GetMaxPrice();

        public Task<List<Shoes>> GetShoesForPayment(List<int> shoesIDs,int userID);
    }
}
