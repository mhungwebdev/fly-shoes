using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database.Query;
using Firebase.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class AuthenController : FlyShoesController<object>
    {
        IFirebaseAuthClient _firebaseService;
        private FirebaseClient _firebaseClient;

        public AuthenController(IFirebaseAuthClient firebaseService)
        {
            _firebaseService = firebaseService;
            _firebaseClient = new FirebaseClient("https://fly-shoes-store-default-rtdb.firebaseio.com");
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
        [Authorize]
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

            var res = result?.AuthCredential?.ProviderType == FirebaseProviderType.EmailAndPassword;

            

            return result;
        }

        [HttpPost("/push-notify")]
        public async Task<IActionResult> PushNotification()
        {
            var notify = new
            {
                NotificationContent = "Đây là thông báo !"
            };

            var result = await _firebaseClient.Child("Notify").PostAsync(notify);

            return Ok(result);
        }
    }
}
