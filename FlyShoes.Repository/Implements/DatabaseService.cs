using Dapper;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
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
    public class DatabaseService<Entity> : IDatabaseService<Entity>
    {
        protected string? connectString;
        public DatabaseService(IConfiguration configuration)
        {
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
            var result = await connection.QueryAsync<List<Entity>>("Proc_user_GetAll",param:param, commandType:CommandType.StoredProcedure);
            return (List<Entity>)result;
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
