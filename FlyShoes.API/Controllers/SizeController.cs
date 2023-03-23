using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class SizeController : FlyShoesController<Size>
    {
        public SizeController(ISizeBL sizeBL):base(sizeBL)
        {

        }
    }
}
