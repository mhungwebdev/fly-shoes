using Firebase.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlyShoes.Core.Interfaces;
using FlyShoes.DAL.Interfaces;
using FlyShoes.Common.Constants;

namespace FlyShoes.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        IFirebaseAuthClient _firebaseService;
        IFirestoreService _firestoreService;

        public AuthenController(IFirebaseAuthClient firebaseService,IFirestoreService firestoreService)
        {
            _firebaseService = firebaseService;
            _firestoreService = firestoreService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> RegisterAccount(Dictionary<string,object> user)
        {
            var email = user.GetValue("Email").ToString();
            var password = user.GetValue("Password").ToString();
            var result = await _firebaseService.SignInWithEmailAndPasswordAsync(email, password);

            return Ok(result);
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task Logout()
        {
            _firebaseService.SignOut();
        }
    }
}
