using Firebase.Database;
using Firebase.Database.Query;
using FlyShoes.Interfaces;
using FlyShoes.DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FlyShoes.Common.Enums;
using FlyShoes.Common.Constants;

namespace FlyShoes.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FlyShoesController<Entity> : ControllerBase
    {
        IBaseBL<Entity> _baseBL;
        
        public FlyShoesController(IBaseBL<Entity> baseBL)
        {
            _baseBL= baseBL;
        }

        [HttpGet]
        public async Task<ServiceResponse> Get()
        {
            var result = new ServiceResponse();
            result.Data = await _baseBL.GetAll();

            return result;
        }

        [HttpGet("get-by-field")]
        public async Task<ServiceResponse> GetByField(string fieldName,string fieldValue)
        {
            var result = new ServiceResponse();
            result.Data = await _baseBL.GetByField(fieldName,fieldValue);

            return result;
        }

        [HttpGet("{id}")]
        public async Task<ServiceResponse> Get(int id)
        {
            var result = new ServiceResponse();
            result.Data = await _baseBL.GetByID(id.ToString());

            return result;
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_baseBL.ValidateRole(Common.ActionEnum.Delete)) return StatusCode(403);

            var res = await _baseBL.Delete(id.ToString());

            return Ok(res);
        }

        [HttpPost("paging")]
        public virtual async Task<ServiceResponse> Paging(PagingPayload pagingPayload)
        {
            var result = new ServiceResponse();

            result.Data = await _baseBL.Paging(pagingPayload);

            return result;
        }

        [HttpPost("total")]
        public async Task<ServiceResponse> GetTotal(PagingPayload pagingPayload)
        {
            var result = new ServiceResponse();

            result = await _baseBL.GetTotal(pagingPayload);

            return result;
        }

        [HttpPost("save")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Save(Entity entity)
        {
            var state = entity.GetValue<ModelStateEnum>("State");
            switch (state)
            {
                case ModelStateEnum.None:
                    break;
                case ModelStateEnum.Insert:
                    if (!_baseBL.ValidateRole(Common.ActionEnum.Insert)) return StatusCode(403);
                    break;
                case ModelStateEnum.Update:
                    if (!_baseBL.ValidateRole(Common.ActionEnum.Update)) return StatusCode(403);
                    break;
                case ModelStateEnum.Delete:
                    break;
                case ModelStateEnum.Duplicate:
                    break;
            }

            var result = await _baseBL.Save(entity);

            return Ok(result);
        }

        [HttpDelete("delete-multi")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteMulti([FromBody]List<int> ids)
        {
            if (!_baseBL.ValidateRole(Common.ActionEnum.DeleteMulti)) return StatusCode(403);

            var res = await _baseBL.DeleteMulti(ids);

            return Ok(res);
        }

        [HttpPost("save-list")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = RoleTypeConstant.ADMIN)]
        public async Task<ServiceResponse> Save(List<Entity> entities)
        {
            var result = await _baseBL.SaveList(entities);

            return result;
        }

        [HttpPost("update-field/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleTypeConstant.CUSTOMER)]
        public async Task<IActionResult> UpdateSingleField(int id,[FromBody]InfoUpdateField updateInfo)
        {
            if (!_baseBL.ValidateRole(Common.ActionEnum.Update)) return StatusCode(403);

            return Ok(await _baseBL.UpdateSingleField(updateInfo, id.ToString()));
        }
    }
}
