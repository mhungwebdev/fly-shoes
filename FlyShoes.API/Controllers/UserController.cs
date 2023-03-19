using FlyShoes.Common.Models;
using FlyShoes.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class UserController : FlyShoesController<User>
    {
        public UserController(IFirestoreService firestoreService):base(firestoreService)
        {

        }
    }
}
