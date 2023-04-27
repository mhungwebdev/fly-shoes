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
        IFirestoreService _firestoreService;

        public UserController(IUserBL userBL,IFirestoreService firestoreService) : base(userBL)
        {
            _userBL = userBL;
            _firestoreService = firestoreService;
        }

        [HttpPost("start")]
        public async Task<ServiceResponse> GetStart(User user)
        {
            var result = new ServiceResponse();

            user.IsAdmin = false;
            user.AmountSpent = 0;
            user.State = ModelStateEnum.Insert;
            user.IsUsePassword = true;

            await _userBL.Save(user);
            var users = await _userBL.GetByField("FirebaseID", user.FirebaseID);
            await _firestoreService.PushNotification(new Notification()
            {
                Message = "Chào mừng bạn đến với Fly Shoes.",
                UserID = users.FirstOrDefault().UserID
            });

            result.Data = users;
            return result;
        }

        [HttpPost("start-with-social")]
        public async Task<ServiceResponse> StartWithSocial(User user)
        {
            var result = new ServiceResponse();

            var userExsist = await _userBL.GetByField("FirebaseID", user.FirebaseID);
            if(userExsist != null & userExsist.Count > 0)
            {
                result.Data = userExsist;
            }
            else
            {
                user.IsAdmin = false;
                user.AmountSpent = 0;
                user.State = ModelStateEnum.Insert;

                await _userBL.Save(user);
                var users = await _userBL.GetByField("FirebaseID", user.FirebaseID);
                await _firestoreService.PushNotification(new Notification()
                {
                    Message = "Chào mừng bạn đến với Fly Shoes.",
                    UserID = users.FirstOrDefault().UserID
                });
                result.Data = users;
            }

            return result;
        }
    }
}
