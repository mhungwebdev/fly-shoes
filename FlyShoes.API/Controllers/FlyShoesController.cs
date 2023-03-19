using Firebase.Database;
using Firebase.Database.Query;
using FlyShoes.DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FlyShoesController<Entity> : ControllerBase
    {
        IFirestoreService _fireStoreService;

        public FlyShoesController(IFirestoreService firestoreService)
        {
            _fireStoreService = firestoreService;
        }

        [HttpPost("/push-notification")]
        public async Task<IActionResult> PushNotification([FromBody]string message)
        {
            _fireStoreService.PushNotification(1, message, "");

            return Ok();
        }
    }
}
