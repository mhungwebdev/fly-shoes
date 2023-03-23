﻿using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class BrandController : FlyShoesController<Brand>
    {
        public BrandController(IBrandBL brandBL):base(brandBL)
        {

        }
    }
}
