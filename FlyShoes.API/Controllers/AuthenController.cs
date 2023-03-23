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

        [HttpGet("/logout")]
        [Authorize]
        public async Task Logout()
        {
            _firebaseService.SignOut();
        }
    }
}
