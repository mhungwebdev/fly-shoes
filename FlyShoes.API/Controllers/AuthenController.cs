//using FlyShoes.DAL.Interfaces;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        public async Task<UserCredential> RegisterAccount()
        {
            var email = "mhung.haui.webdev@gmail.com";
            var password = "123456";
            var result = await _firebaseService.CreateUserWithEmailAndPasswordAsync(email, password);

            return result;
        }

        [HttpGet("/getUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<User> GetCurrentUser()
        {
            var result = _firebaseService.User;

            return result;
        }

        [HttpGet("/logout")]
        public async Task Logout()
        {
            _firebaseService.SignOut();
        }

        [HttpPost("/login")]
        public async Task<UserCredential> Login([FromBody] Dictionary<string,object> useLogin)
        {
            var email = useLogin.GetValueOrDefault("Email").ToString();
            var password = useLogin.GetValueOrDefault("Password").ToString();
            var result = await _firebaseService.SignInWithEmailAndPasswordAsync(email, password);

            return result;
        }
    }
}
