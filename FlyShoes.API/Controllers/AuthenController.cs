//using FlyShoes.DAL.Interfaces;
using Firebase.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        IFirebaseAuthClient _firebaseService;

        public AuthenController(IFirebaseAuthClient firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [HttpGet]
        public async Task<UserCredential> RegisterAccount(string email,string password)
        {
            var result = await _firebaseService.CreateUserWithEmailAndPasswordAsync(email, password);

            return result;
        }
    }
}
