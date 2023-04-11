using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlyShoes.Common.Models;
using FlyShoes.Common;

namespace FlyShoes.Interfaces
{
    public interface IBaseBL<Entity>
    {
        Task<ServiceResponse> Delete(string id);

        Task<List<Entity>> GetByField(string fieldName,string fieldValue);
        Task<ServiceResponse> UpdateSingleField(InfoUpdateField updateInfo,string id);

        Task<List<Entity>> GetAll();

        Task<Entity> GetByID(string id);

        Task<List<Entity>> Paging(PagingPayload pagingPayLoad);
        Task<ServiceResponse> GetTotal(PagingPayload pagingPayLoad);

        Task<ServiceResponse> Save(Entity entity);

        Task<ServiceResponse> SaveList(List<Entity> entities);

        Task<ServiceResponse> DeleteMulti(List<int> ids);

        public bool ValidateRole(ActionEnum action);
    }
}
