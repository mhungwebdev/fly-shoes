using FlyShoes.Common.Models;
using FlyShoes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.BL.Interfaces
{
    public interface ICartDetailBL : IBaseBL<CartDetail>
    {
        Task<List<CartDetail>> GetCartDetailByUser(int userID);
    }
}
