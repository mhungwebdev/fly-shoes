﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlyShoesController<Entity> : ControllerBase
    {
        public FlyShoesController()
        {

        }

        //[HttpGet]
        //public async Task<Entity> GetByID(string id)
        //{
        //    dynamic obj= null;
        //    return obj;
        //}
    }
}
