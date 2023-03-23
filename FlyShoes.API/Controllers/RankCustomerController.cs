using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class RankCustomerController : FlyShoesController<RankCustomer>
    {
        public RankCustomerController(IRankCustomerBL rankCustomerBL):base(rankCustomerBL)
        {

        }
    }
}
