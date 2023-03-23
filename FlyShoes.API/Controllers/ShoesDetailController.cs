using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class ShoesDetailController : FlyShoesController<ShoesDetail>
    {
        public ShoesDetailController(IShoesDetailBL shoesDetailBL):base(shoesDetailBL)
        {

        }
    }
}
