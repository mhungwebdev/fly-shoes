using FlyShoes.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace FlyShoes.Core.Interfaces
{
    public interface IDatabaseService<Entity>
    {
        /// <summary>
        /// Thêm mới một
        /// </summary>
        /// <param name="entity">entity muốn thêm mới</param>
        /// <returns></returns>
        Task<Entity> Insert(Entity entity);

        /// <summary>
        /// Update bản ghi
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Entity> Update(Entity entity, string id);

        /// <summary>
        /// Xóa bản ghi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(string id);

        /// <summary>
        /// Lấy tất cả bản ghi
        /// </summary>
        /// <returns></returns>
        Task<List<Entity>> GetAll();

        /// <summary>
        /// Lấy bản ghi qua id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Entity> GetByID(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagingPayLoad"></param>
        /// <returns></returns>
        Task<List<Entity>> Paging(PagingPayload pagingPayLoad);
    }
}
