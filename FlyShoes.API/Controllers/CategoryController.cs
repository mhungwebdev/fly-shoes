using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class CategoryController : FlyShoesController<Category>
    {
        public CategoryController(ICategoryBL categoryBL):base(categoryBL)
        {

        }
    }
}
