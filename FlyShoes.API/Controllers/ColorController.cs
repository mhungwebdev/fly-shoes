﻿using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class ColorController : FlyShoesController<Color>
    {
        public ColorController(IColorBL colorBL):base(colorBL)
        {

        }
    }
}
