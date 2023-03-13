using FlyShoes.Common.Models;
using FlyShoes.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.BL
{
    public class BaseBL<Entity> : IBaseBL<Entity>
    {
        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Entity>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Entity> GetByID(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Entity> Insert(Entity entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<Entity>> Paging(PagingPayload pagingPayLoad)
        {
            throw new NotImplementedException();
        }

        public Task<Entity> Update(Entity entity, string id)
        {
            throw new NotImplementedException();
        }
    }
}
