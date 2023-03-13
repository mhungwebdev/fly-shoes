using Dapper;
using FlyShoes.Common.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Repository.Base
{
    public class BaseRepository<Entity> : IBaseRepository<Entity>
    {
        protected string connectString;
        public BaseRepository(IConfiguration configuration) {
            connectString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection GetDbConnection()
        {
            return new MySqlConnection(connectString);
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Entity>> GetAll()
        {
            var connection = GetDbConnection();
            connection.Open();
            var param = new Dictionary<string, object>()
            {
                {"UserID",2 }
            };

            var result = connection.QuerySingle<List<Entity>>("Proc_user_GetAll",param:param, commandType:CommandType.StoredProcedure).ToList();

            return result;
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
