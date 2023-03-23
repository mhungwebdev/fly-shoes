using Firebase.Database;
using Firebase.Database.Query;
using FlyShoes.Interfaces;
using FlyShoes.DAL.Interfaces;
using FlyShoes.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

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

        #region Get
        /// <summary>
        /// Lấy tất cả bản ghi
        /// Author : mhungwebdev (28/8/2022)
        /// </summary>
        /// <returns>Tất cả bản ghi</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var res = _baseBL.GetAll();

            return Ok(res);
        }
        #endregion

        #region Get by id
        /// <summary>
        /// Lấy theo id
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="id">id của bản ghi</param>
        /// <returns>Bản ghi có id tương ứng</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var res = _baseBL.GetByID(id.ToString());

            return Ok(res);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Xóa theo id
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="id">id bản ghi muốn xóa</param>
        /// <returns>1 nếu thành công</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var res = _baseBL.Delete(id.ToString());

            return Ok(res);
        }
        #endregion

        #region Insert
        /// <summary>
        /// Thêm mới 1 bản ghi
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="entity">bản ghi mới</param>
        /// <returns>1 nếu thành công</returns>
        [HttpPost]
        public IActionResult Insert(Entity entity)
        {
            var res = _baseBL.Insert(entity);

            return StatusCode(201, res);
        }
        #endregion

        #region Update
        /// <summary>
        /// Sửa bản ghi
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="entity">bản ghi cập nhật</param>
        /// <param name="id">id bản ghi cập nhật</param>
        /// <returns>1 nếu update thành công</returns>
        [HttpPut("{id}")]
        public virtual IActionResult Update(Entity entity, int id)
        {
            var res = _baseBL.Update(entity, id.ToString());

            return StatusCode(200, res);
        }
        #endregion
    }

}
