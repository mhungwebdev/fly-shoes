using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class ShoesController : FlyShoesController<Shoes>
    {
        public ShoesController(IShoesBL shoesBL):base(shoesBL)
        {

        }
    }
}
