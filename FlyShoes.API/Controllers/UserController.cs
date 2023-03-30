using Firebase.Auth;
using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Enums;
using FlyShoes.Common.Models;
using FlyShoes.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class UserController : FlyShoesController<Common.Models.User>
    {
        private IUserBL _userBL;
        public UserController(IUserBL userBL) : base(userBL)
        {
            _userBL = userBL;
        }

        [HttpPost("start")]
        public async Task<ServiceResponse> GetStart(Dictionary<string, object> userFirebase)
        {
            var result = new ServiceResponse();
            var firebaseID = userFirebase["uid"];
            var email = userFirebase["email"];

            var newUser = new Common.Models.User()
            {
                Email = email.ToString(),
                FirebaseID = firebaseID.ToString(),
                IsAdmin = false,
                AmountSpent = 0,
                State = ModelStateEnum.Insert,
                FullName = email.ToString()
            };

            await _userBL.Save(newUser);
            result.Data = await _userBL.GetByField("FirebaseID", firebaseID.ToString());


            return result;
        }
    }
}
