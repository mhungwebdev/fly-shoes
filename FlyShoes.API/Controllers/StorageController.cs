using FlyShoes.Common.Constants;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyShoes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        IStorageService _storageService;

        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("/upload")]
        public async Task<IActionResult> UploadFile(List<IFormFile> files)
        {
            var fileUploads = files.Select(file => new FSFile(file)).ToList();


            var res = await _storageService.UploadFileMulti(fileUploads, Common.Enums.BucketEnum.TempBucket);

            return Ok(res);
        }

        [HttpPut("/tem-to-main")]
        public async Task<IActionResult> MoveFileTemToMain(Guid fileID)
        {
            var res = await _storageService.MoveFileFromTemToMain(fileID);

            return Ok(res);
        }

        [HttpDelete("/delete")]
        public async Task<IActionResult> DeleteFile(List<Guid> fileIDs)
        {
            await _storageService.DeleteFileMulti(fileIDs); 
            return Ok();
        }
    }
}
