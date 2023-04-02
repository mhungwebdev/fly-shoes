using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class ShoesController : FlyShoesController<Shoes>
    {
        IShoesBL _shoesBL;
        public ShoesController(IShoesBL shoesBL):base(shoesBL)
        {
            _shoesBL = shoesBL;
        }

        [HttpGet("max-price")]
        public async Task<ServiceResponse> GetMaxPrice()
        {
            var response = new ServiceResponse();

            response.Data = await _shoesBL.GetMaxPrice();

            return response;
        }
    }
}
