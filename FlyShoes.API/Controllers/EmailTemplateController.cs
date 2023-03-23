using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    public class EmailTemplateController : FlyShoesController<EmailTemplate>
    {
        public EmailTemplateController(IEmailTemplateBL emailTemplateBL):base(emailTemplateBL)
        {

        }
    }
}
