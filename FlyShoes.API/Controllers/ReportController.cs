using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Constants;
using FlyShoes.Common.Enums;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FlyShoes.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        IDatabaseService _databaseService;
        IReportBL _reportBL;

        public ReportController(IDatabaseService databaseService,IReportBL reportBL)
        {
            _databaseService = databaseService;
            _reportBL = reportBL;
        }

        [HttpGet("GetReport/{tab}/{timeAnalyst}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleTypeConstant.ADMIN)]
        public async Task<ServiceResponse> GetReport(TabAnalyst tab,TimeToAnalyst timeAnalyst)
        {
            var result = new ServiceResponse();

            result.Data = await _reportBL.GetReport(tab,timeAnalyst);

            return result;
        }
    }
}
