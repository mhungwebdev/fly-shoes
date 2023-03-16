using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FlyShoesController<Entity> : ControllerBase
    {
        public FlyShoesController()
        {

        }

        [HttpGet]
        public async Task<IActionResult> GetByID(string id)
        {
            dynamic obj = null;
            return Ok(null);
        }
    }
}
