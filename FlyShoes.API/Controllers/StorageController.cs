using FlyShoes.Common.Constants;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = RoleTypeConstant.ADMIN)]
    public class StorageController : ControllerBase
    {
        IStorageService _storageService;

        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("upload-multi")]
        public async Task<IActionResult> UploadFileMulti(List<IFormFile> files)
        {
            var fileUploads = files.Select(file => new FSFile(file)).ToList();


            var res = await _storageService.UploadFileMulti(fileUploads, Common.Enums.BucketEnum.TempBucket);

            return Ok(res);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile files)
        {
            var fsFile = new FSFile(files);
            var res = await _storageService.UploadFile(fsFile, Common.Enums.BucketEnum.TempBucket);

            return Ok(res);
        }

        [HttpPut("tem-to-main")]
        public async Task<IActionResult> MoveFileTemToMain(Guid fileID)
        {
            var res = await _storageService.MoveFileFromTemToMain(fileID);

            return Ok(res);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFile(List<Guid> fileIDs)
        {
            await _storageService.DeleteFileMulti(fileIDs); 
            return Ok();
        }
    }
}
