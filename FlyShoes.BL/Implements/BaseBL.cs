using FlyShoes.BL.Interfaces;
using FlyShoes.Common;
using FlyShoes.Common.Constants;
using FlyShoes.Common.Enums;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using FlyShoes.Interfaces;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;

namespace FlyShoes.BL
{
    public class BaseBL<Entity> : IBaseBL<Entity>
    {
        protected IDatabaseService _dataBaseService;
        private static string _tableName;
        private static List<string> _fieldSearchs;
        private static bool _isMater;
        private static PropertyInfo _primaryKeyPropertyInfor;
        private static string _fieldPrimaryKey;
        private static List<string> _tableDetail;

        public static readonly DateFormatHandling JSONDateFormatHandling = DateFormatHandling.IsoDateFormat;
        public static readonly DateTimeZoneHandling JSONTimeZoneHandling = DateTimeZoneHandling.Local;
        public static readonly string JSONDateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffk";
        public static readonly NullValueHandling JSONNullValueHandling = NullValueHandling.Include;
        public static readonly ReferenceLoopHandling JSONReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        public static JsonSerializerSettings _jsonSerializerSettings;

        public BaseBL(IDatabaseService databaseService)
        {
            _dataBaseService = databaseService;
            _jsonSerializerSettings = new JsonSerializerSettings() { 
                DateFormatHandling = JSONDateFormatHandling,
                DateTimeZoneHandling = JSONTimeZoneHandling,
                DateFormatString = JSONDateFormatString,
                NullValueHandling = JSONNullValueHandling,
                ReferenceLoopHandling = JSONReferenceLoopHandling,
                //ContractResolver = new ContractRe
            };
           
            SetupBL();
        }

        #region SetupBL
        public void SetupBL()
        {
            var configTable = typeof(Entity).GetCustomAttributes(typeof(ConfigTable), true).FirstOrDefault();
            var primaryKey = typeof(Entity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PrimaryKey))).FirstOrDefault();
            if (primaryKey != null)
            {
                _primaryKeyPropertyInfor = primaryKey;
                _fieldPrimaryKey = primaryKey.Name;
            }
            else
            {
                throw new FSException("Chưa setup khóa chính cho model");
            }
            if (configTable != null)
            {
                _tableName = (configTable as ConfigTable).TableName;
                _fieldSearchs = (configTable as ConfigTable).FieldSearch;
                _isMater = (configTable as ConfigTable).IsMaster;
                _tableDetail = (configTable as ConfigTable).DetailTables;
            }
            else
            {
                throw new FSException("Chưa thêm attribute config table");
            }
        }
        #endregion

        #region Delete
        public async Task<ServiceResponse> Delete(string id)
        {
            var result = new ServiceResponse();
            var validates = ValidateBeforeDelete(id);

            if(validates != null && validates.Count > 0)
            {
                var isUsedValidate = validates.Find(validate => validate.ErrorCode == ErrorCodeEnum.IsUsed) != null ? true : false;
                result.ValidateInfo = validates;
                result.Success = false;
                result.ErrorCode = isUsedValidate ? ErrorCodeEnum.IsUsed : ErrorCodeEnum.NoError;
                return result;
            }
            IDbConnection connection = _dataBaseService.GetDbConnection();
            connection.Open();
            IDbTransaction transaction = connection.BeginTransaction();

            result.Data = DoDelete(id,connection,transaction);

            AfterDelete(id,connection,transaction);

            transaction.Commit();
            transaction.Dispose();
            connection.Dispose();
            connection.Close();
            AfterCommitDelete(id);

            return result;
        }
        #endregion

        #region Get ALL
        public async Task<List<Entity>> GetAll()
        {
            var command = GetProcGetALL();
            var result = await _dataBaseService.QueryUsingStoredProcedureAsync<Entity>(command);

            return result.ToList();
        }
        #endregion

        #region Get By ID
        public async Task<Entity> GetByID(string id)
        {
            var command = GetProcGetByID();
            var param = new Dictionary<string, object>()
            {
                {"v_ID", id}
            };
            var result = await _dataBaseService.QuerySingleUsingStoredProcedureAsync<Entity>(command, param);

            var detailAttributes = typeof(Entity).GetCustomAttributes(typeof(Detail));
            foreach(var detailAttribute in detailAttributes)
            {
                var commandGetDetail = (detailAttribute as Detail).CommandGetDetail;
                var property = (detailAttribute as Detail).PropertyInMaster;
                var typeDetail = (detailAttribute as Detail).Type;

                var paramGetDetail = new Dictionary<string, object>()
                {
                    {"@MasterID",id }
                };

                var detail = await _dataBaseService.QueryUsingCommanTextAsync(commandGetDetail, paramGetDetail);
                result.SetValue(property, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(detail,_jsonSerializerSettings), typeDetail, _jsonSerializerSettings));
            }

            GetMoreInfo(result);

            return result;
        }
        #endregion

        public virtual void GetMoreInfo(object entity)
        {

        }

        #region Paging
        public async Task<List<Entity>> Paging(PagingPayload pagingPayLoad)
        {
            var whereClause = BuildWhereClause(pagingPayLoad.FilterColumns);
            var searhClause = BuidSearchClause(pagingPayLoad.Keyword);
            var procedureName = GetProcPaging();
            var sortOrder = BuildSortOrder(pagingPayLoad.SortOrder);

            var param = new Dictionary<string, object>() {
                {"v_WhereClause",whereClause },
                {"v_SearchClause",searhClause },
                {"v_PageSize",pagingPayLoad.PageSize },
                {"v_PageIndex",pagingPayLoad.PageIndex },
                {"v_SortOrder", sortOrder},
                {"v_Table", _tableName}
            };

            var result = _dataBaseService.QueryUsingStoredProcedure<Entity>(procedureName, param);

            return result;
        }
        #endregion

        #region Get proc
        public string GetProcGetALL()
        {
            return $"Proc_{_tableName}_GetALL"; ;
        }

        public string GetProcGetByID()
        {
            return $"Proc_{_tableName}_GetByID";
        }

        public string GetProcPaging()
        {
            return $"Proc_Paging";
        }

        public string GetProcInsert()
        {
            return $"Proc_{_tableName}_Insert";
        }

        public string GetProcUpdate()
        {
            return $"Proc_{_tableName}_Update";
        }
        #endregion

        #region Build paging
        public string BuidSearchClause(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword) || _fieldSearchs == null || _fieldSearchs.Count == 0) 
                return null;

            var result = "(";

            foreach(var fieldSearch in _fieldSearchs)
            {
                result += $"{fieldSearch} LIKE '%{keyword}%' OR";
            }

            result = result.Substring(0,result.Length - 3) + ")";

            return result;
        }

        public string BuildWhereClause(List<FilterColumn> filterColumns)
        {
            if (filterColumns == null || filterColumns.Count == 0) return null;
            
            var result = "(";

            foreach(var filterColumn in filterColumns)
            {
                if(filterColumn.FilterOperator != FilterOperator.EqualNull && filterColumn.FilterOperator != FilterOperator.NotEqualNull)
                {
                    if(filterColumn.Value != null)
                    {
                        switch (filterColumn.FilterOperator)
                    {
                        case Common.Enums.FilterOperator.Equal:
                            result += $"({filterColumn.FieldName} = {filterColumn.Value}) AND ";
                            break;
                        case Common.Enums.FilterOperator.NotEqual:
                            result += $"({filterColumn.FieldName} <> {filterColumn.Value}) AND ";
                            break;
                        case Common.Enums.FilterOperator.Greater:
                            result += $"({filterColumn.FieldName} > {filterColumn.Value}) AND ";
                            break;
                        case Common.Enums.FilterOperator.Less:
                            result += $"({filterColumn.FieldName} < {filterColumn.Value}) AND ";
                            break;
                        case Common.Enums.FilterOperator.GreaterOrEqual:
                            result += $"({filterColumn.FieldName} > {filterColumn.Value} OR {filterColumn.FieldName} = {filterColumn.Value}) AND ";
                            break;
                        case Common.Enums.FilterOperator.LessOrEqual:
                            result += $"({filterColumn.FieldName} < {filterColumn.Value} OR {filterColumn.FieldName} = {filterColumn.Value}) AND ";
                            break;
                    }
                    }
                }
                else
                {
                    var not = filterColumn.FilterOperator == FilterOperator.NotEqualNull ? "NOT" : "";
                    result += $"({filterColumn.FieldName} IS {not} NULL) AND ";
                }

            }
            result = result.Substring(0, result.Length - 5);
            result += ")";

            return result;
        }

        public string BuildSortOrder(SortOrder sortOrder)
        {
            if (sortOrder == null && string.IsNullOrEmpty(sortOrder.FieldSort)) return null;

            var sortType = SortConstant.DESC;
            switch (sortOrder.SortType)
            {
                case Common.Enums.SortType.ASC:
                    sortType = SortConstant.ASC;
                    break;
                case Common.Enums.SortType.DESC:
                    sortType = SortConstant.DESC;
                    break;
            }

            var result = $"{sortOrder.FieldSort} {sortType}";

            return result;
        }
        #endregion

        #region Flow delete
        public virtual List<ValidateResult> ValidateBeforeDelete(string id)
        {
            var result = new List<ValidateResult>();

            if(_tableDetail != null && _tableDetail.Count > 0)
            {
                foreach(var tableRelated in _tableDetail)
                {
                    var checkRelatedTable = $"SELECT COUNT(1) FROM {tableRelated} WHERE {_fieldPrimaryKey} = @ID";
                    var param = new Dictionary<string, object>()
                    {
                        {"@ID",id }
                    };

                    var isUsed = _dataBaseService.ExecuteScalarUsingCommandText<int>(checkRelatedTable, param) > 0;
                    if(isUsed)
                    {
                        result.Add(new ValidateResult()
                        {
                            ErrorCode = ErrorCodeEnum.IsUsed,
                            FieldError = _fieldPrimaryKey
                        });
                    }
                }
            }

            return result;
        }

        public virtual int DoDelete(string id,IDbConnection connection,IDbTransaction transaction)
        {
            var commandDelete = $"DELETE FROM {_tableName} WHERE {_fieldPrimaryKey} = @ID;";
            
            var detailDeletes = string.Empty;
            if(_tableDetail != null && _tableDetail.Count > 0)
            {
                foreach(var table in _tableDetail)
                {
                    detailDeletes += $"DELETE FROM {table} WHERE {_fieldPrimaryKey} = @ID;";
                }
            }

            var param = new Dictionary<string, object>()
            {
                {"@ID",id }
            };
            return _dataBaseService.ExecuteUsingCommandText(commandDelete+detailDeletes,param,transaction,connection);
        }

        public virtual void AfterDelete(string id,IDbConnection connection,IDbTransaction transaction)
        {

        }

        public virtual void AfterCommitDelete(string id)
        {

        }
        #endregion

        #region Save
        public async Task<ServiceResponse> Save(Entity entity)
        {
            var result = new ServiceResponse();
            result.Success = false;
            var validates = ValidateBeforeSave(entity);

            if(validates != null && validates.Count > 0)
            {
                result.ValidateInfo = validates;
                result.Success = false;

                return result;
            }

            BeforeSave(entity);

            var connection = _dataBaseService.GetDbConnection();
            connection.Open();
            var transaction = connection.BeginTransaction();

            result.Success = DoSave(entity,connection,transaction) > 0;

            AfterSave(entity, connection, transaction);

            transaction.Commit();
            transaction.Dispose();
            connection.Dispose();
            connection.Close();
            result.Success = true;

            AfterCommitSave(entity);

            return result;
        }
        #endregion

        #region Save List
        public async Task<ServiceResponse> SaveList(List<Entity> entities)
        {
            var result = new ServiceResponse();
            var validates = ValidateSaveList(entities);
            if(validates != null && validates.Count > 0)
            {
                result.Success = false;
                result.ValidateInfo = validates;

                return result;
            }

            foreach(var entity in entities)
            {
                BeforeSave(entity);
            }

            var entityInserts = entities.Where(entity => entity.GetValue<ModelStateEnum>("State").Equals(ModelStateEnum.Insert))?.ToList();
            var entityDeletes = entities.Where(entity => entity.GetValue<ModelStateEnum>("State").Equals(ModelStateEnum.Delete))?.ToList();
            var entityUpdates = entities.Where(entity => entity.GetValue<ModelStateEnum>("State").Equals(ModelStateEnum.Update))?.ToList();

            var connection = _dataBaseService.GetDbConnection();
            connection.Open();
            var transaction = connection.BeginTransaction();

            var quantityDelete = DoDeleteMulti(entityDeletes.Select(entity => entity.GetPrimaryKey<Entity>()).ToList(),connection,transaction);
            var quantityInsert = DoInsertMulti(entityInserts, connection,transaction);
            var quantityUpdate = DoUpdateMulti(entityUpdates, connection, transaction);

            AfterDeleteMulti(entities,connection,transaction);
            AfterInsertMulti(entities,connection,transaction);
            AfterUpdateMulti(entities, connection, transaction);

            transaction.Commit();
            transaction.Dispose();
            connection.Dispose();
            connection.Close();

            AfterDeleteMultiCommit(entities);
            AfterInsertMultiCommit(entities);
            AfterUpdateMultiCommit(entities);

            result.Data = new
            {
                QuantityDelete = quantityDelete,
                QuantityInsert = quantityInsert,
                QuantityUpdate = quantityUpdate
            };

            return result;
        }

        public List<ValidateResult> ValidateSaveList(List<Entity> entities)
        {
            var result = new List<ValidateResult>();
            foreach(var entity in entities)
            {
                var validates = ValidateBeforeSave(entity);
                result.Concat(validates);
            }

            return result;
        }

        public async Task<ServiceResponse> DeleteMulti(List<int> ids)
        {
            var result = new ServiceResponse();
            var validates = new List<ValidateResult>();

            foreach(var id in ids)
            {
                var validate = ValidateBeforeDelete(id.ToString());
                validates.Concat(validate);
            }

            if(validates != null && validates.Count > 0)
            {
                result.Success = false;
                result.ValidateInfo = validates;
                return result;
            }

            var connection = _dataBaseService.GetDbConnection();
            connection.Open();
            var transaction = connection.BeginTransaction();

            result.Data = DoDeleteMulti(ids,connection,transaction);

            transaction.Commit();
            transaction.Dispose();
            connection.Dispose();
            connection.Close();

            return result;
        }

        public int DoDeleteMulti(List<int> ids,IDbConnection connection,IDbTransaction transaction)
        {
            var commandDeleteMulti = $"DELETE FORM {_tableName} WHERE {_fieldPrimaryKey} IN " + "{0}";

            return _dataBaseService.ExecuteUsingCommandText(string.Format(commandDeleteMulti, string.Join(",",ids)),transaction:transaction,connection:connection);
        }

        public int DoInsertMulti(List<Entity> entities, IDbConnection connection, IDbTransaction transaction)
        {
            var propInserts = typeof(Entity).GetProperties().Where(prop => !Attribute.IsDefined(prop, typeof(NotMap)));
            var commandInsert = $"INSERT INTO {_tableName} (" + "{0}) VALUES ";
            commandInsert = string.Format(commandInsert,string.Join(",",propInserts.Select(prop => prop.Name)));

            var index = 0;
            var param = new Dictionary<string, object>();
            foreach(var entity in entities)
            {
                var valueInsert = "(";
                foreach(var prop in propInserts)
                {
                    valueInsert += $"${prop.Name}_{index},";
                    param.Add($"{prop.Name}_{index}", prop.GetValue(entity));
                }
                valueInsert = valueInsert.Substring(0,valueInsert.Length - 1) + "),";
                commandInsert += valueInsert;
                index++;
            }
            commandInsert = commandInsert.Substring(0,commandInsert.Length - 1) + ";";

            return _dataBaseService.ExecuteUsingCommandText(commandInsert,param,transaction,connection);
        }

        public int DoInsertMulti<Entity>(List<Entity> entities, IDbConnection connection, IDbTransaction transaction)
        {
            var propInserts = typeof(Entity).GetProperties().Where(prop => !Attribute.IsDefined(prop, typeof(NotMap)));
            var tableName = ((typeof(Entity).GetCustomAttributes(typeof(ConfigTable), true).FirstOrDefault()) as ConfigTable).TableName;
            var commandInsert = $"INSERT INTO {tableName} (" + "{0}) VALUES ";
            commandInsert = string.Format(commandInsert, string.Join(",", propInserts.Select(prop => prop.Name)));

            var index = 0;
            var param = new Dictionary<string, object>();
            foreach (var entity in entities)
            {
                var valueInsert = "(";
                foreach (var prop in propInserts)
                {
                    valueInsert += $"@{prop.Name}_{index},";
                    param.Add($"@{prop.Name}_{index}", prop.GetValue(entity));
                }
                valueInsert = valueInsert.Substring(0, valueInsert.Length - 1) + "),";
                commandInsert += valueInsert;
                index++;
            }
            commandInsert = commandInsert.Substring(0, commandInsert.Length - 1) + ";";

            return _dataBaseService.ExecuteUsingCommandText(commandInsert, param, transaction, connection);
        }
        public int DoUpdateMulti(List<Entity> entities, IDbConnection connection, IDbTransaction transaction)
        {
            var propUpdates = typeof(Entity).GetType().GetProperties().Where(prop => !Attribute.IsDefined(prop,typeof(NotMap)) && !Attribute.IsDefined(prop,typeof(PrimaryKey)));

            var commandUpdates = string.Empty;
            var index = 0;
            var param = new Dictionary<string, object>();
            foreach (var entity in entities)
            {
                var commandUpdate = $"UPDATE {_tableName} SET ";
                foreach (var prop in propUpdates)
                {
                    commandUpdate += $"{prop.Name} = {prop.Name}_{index},";
                    param.Add($"{prop.Name}_{index}", prop.GetValue(entity));
                }

                commandUpdate = commandUpdate.Substring(0, commandUpdate.Length - 1) + $" WHERE {_primaryKeyPropertyInfor.Name} = @ID_{index};";
                param.Add($"@ID_{index}", entity.GetPrimaryKey<Entity>());
                commandUpdates += commandUpdate;
                index++;
            }

            return _dataBaseService.ExecuteUsingCommandText(commandUpdates, param, transaction, connection);
        }

        public virtual void AfterDeleteMulti(List<Entity> entities,IDbConnection connection,IDbTransaction transaction)
        {

        }
        public virtual void AfterInsertMulti(List<Entity> entities, IDbConnection connection, IDbTransaction transaction)
        {

        }
        public virtual void AfterUpdateMulti(List<Entity> entities, IDbConnection connection, IDbTransaction transaction)
        {

        }

        public virtual void AfterDeleteMultiCommit(List<Entity> entities)
        {

        }
        public virtual void AfterInsertMultiCommit(List<Entity> entities)
        {

        }
        public virtual void AfterUpdateMultiCommit(List<Entity> entities)
        {

        }
        #endregion

        #region Validate Func
        public virtual List<ValidateResult> ValidateEmail(Entity entity)
        {
            var result = new List<ValidateResult>();
            var emailProps = typeof(Entity).GetProperties(typeof(Email));
            
            if(emailProps != null && emailProps.Count > 0)
            {
                foreach(var emailProp in emailProps)
                {
                    var email = emailProp.GetValue(entity).ToString();
                    if (!string.IsNullOrWhiteSpace(email) && !email.ValidateEmail())
                    {
                        result.Add(new ValidateResult()
                        {
                            ErrorCode = ErrorCodeEnum.EmailInValid,
                            FieldError = emailProp.Name
                        });
                    }
                }
            }

            return result;
        }

        public virtual List<ValidateResult> ValidatePhone(Entity entity)
        {
            var result = new List<ValidateResult>();
            var phoneProps = typeof(Entity).GetProperties(typeof(Phone));

            if (phoneProps != null && phoneProps.Count > 0)
            {
                foreach (var phoneProp in phoneProps)
                {
                    var phone = phoneProp.GetValue(entity)?.ToString();
                    if (!string.IsNullOrWhiteSpace(phone) && !phone.ValidatePhone())
                    {
                        result.Add(new ValidateResult()
                        {
                            ErrorCode = ErrorCodeEnum.PhoneInValid,
                            FieldError = phoneProp.Name
                        });
                    }
                }
            }

            return result;
        }

        public virtual List<ValidateResult> ValidateUnique(Entity entity)
        {
            var result = new List<ValidateResult>();
            var uniqueProps = typeof(Entity).GetProperties(typeof(Unique));

            if (uniqueProps != null && uniqueProps.Count > 0)
            {
                foreach (var uniqueProp in uniqueProps)
                {
                    var value = uniqueProp.GetValue(entity).ToString();
                    if (!string.IsNullOrWhiteSpace(value) && CheckExitByField(uniqueProp.Name,value))
                    {
                        result.Add(new ValidateResult()
                        {
                            ErrorCode = ErrorCodeEnum.UniqueInValid,
                            FieldError = uniqueProp.Name
                        });
                    }
                }
            }

            return result;
        }

        public virtual List<ValidateResult> ValidateLength(Entity entity)
        {
            var result = new List<ValidateResult>();
            var lengthProps = typeof(Entity).GetProperties(typeof(Length));

            if (lengthProps != null && lengthProps.Count > 0)
            {
                foreach (var uniqueProp in lengthProps)
                {
                    var value = uniqueProp.GetValue(entity)?.ToString();
                    var maxLength = (uniqueProp.GetCustomAttribute(typeof(Length)) as Length).MaxLength;
                    if (!string.IsNullOrWhiteSpace(value) && value.Length > maxLength)
                    {
                        result.Add(new ValidateResult()
                        {
                            ErrorCode = ErrorCodeEnum.MaxLengthInValid,
                            FieldError = uniqueProp.Name
                        });
                    }
                }
            }

            return result;
        }

        public virtual List<ValidateResult> ValidateRequired(Entity entity)
        {
            var result = new List<ValidateResult>();
            var requriedProps = typeof(Entity).GetProperties(typeof(Common.Required));

            if (requriedProps != null && requriedProps.Count > 0)
            {
                foreach (var requriedProp in requriedProps)
                {
                    var value = requriedProp.GetValue(entity)?.ToString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        result.Add(new ValidateResult()
                        {
                            ErrorCode = ErrorCodeEnum.RequiredInValid,
                            FieldError = requriedProp.Name
                        });
                    }
                }
            }

            return result;
        }
        #endregion

        public bool CheckExitByField(string fieldName,string fieldValue)
        {
            var commandCheckExist = $"SELECT COUNT(1) FROM {_tableName} WHERE {fieldName} = @Value";
            var param = new Dictionary<string, object>()
            {
                {"@Value",fieldValue }
            };

            return _dataBaseService.ExecuteScalarUsingCommandText<int>(commandCheckExist, param) > 0;
        }


        #region Do Insert
        public virtual int DoInsert(Entity entity, IDbConnection connection, IDbTransaction transaction)
        {
            var procInsert = GetProcInsert();
            var param = BuildParamFromEntity(entity);

            return _dataBaseService.ExecuteScalarUsingStoreProcedure<int>(procInsert, param, transaction,connection);
        }
        #endregion

        #region Do Update
        public virtual int DoUpdate(Entity entity, IDbConnection connection, IDbTransaction transaction)
        {
            var procUpdate = GetProcUpdate();
            var param = BuildParamFromEntity(entity);

            return _dataBaseService.ExecuteUsingStoredProcedure(procUpdate, param, transaction, connection);
        }
        #endregion

        #region Build Param from Entity
        public virtual Dictionary<string,object> BuildParamFromEntity(Entity entity)
        {
            var param = new Dictionary<string,object>();

            var props = typeof(Entity).GetProperties().Where(prop => !Attribute.IsDefined(prop,typeof(NotMap))).ToList();

            foreach(var prop in props)
            {
                var fieldName = prop.Name;
                var fieldValue = prop.GetValue(entity);

                param.Add($"v_{fieldName}", fieldValue);
            }

            return param;
        }
        #endregion

        #region Save Flow
        public virtual List<ValidateResult> ValidateBeforeSave(Entity entity)
        {
            var result = new List<ValidateResult>();

            switch (entity.GetValue<ModelStateEnum>("State"))
            {
                case ModelStateEnum.None:
                    break;
                case ModelStateEnum.Insert:
                case ModelStateEnum.Update:
                    var validateEmail = ValidateEmail(entity);
                    var validatePhone = ValidatePhone(entity);
                    var validateRequired = ValidateRequired(entity);
                    var validateUnique = ValidateUnique(entity);
                    var validateLength = ValidateLength(entity);
                    result.Concat(validateEmail).Concat(validatePhone).Concat(validateRequired).Concat(validateUnique).Concat(validateLength);
                    break;
                case ModelStateEnum.Delete:
                    var validateDelete = ValidateBeforeDelete(entity.GetPrimaryKey<Entity>().ToString());
                    result.Concat(validateDelete);
                    break;
                case ModelStateEnum.Duplicate:
                    break;
            }

            return result;
        }

        public virtual void BeforeSave(Entity entity)
        {
            switch (entity.GetValue<ModelStateEnum>("State"))
            {
                case ModelStateEnum.None:
                    break;
                case ModelStateEnum.Insert:
                    entity.SetValue("CreatedDate", DateTime.Now);
                    entity.SetValue("CreatedBy",_dataBaseService?.CurrentUser?.FullName);
                    entity.SetValue("ModifiedDate", DateTime.Now);
                    entity.SetValue("ModifiedBy", _dataBaseService?.CurrentUser?.FullName);
                    break;
                case ModelStateEnum.Update:
                    entity.SetValue("ModifiedDate", DateTime.Now);
                    entity.SetValue("ModifiedBy", _dataBaseService?.CurrentUser?.FullName);
                    break;
                case ModelStateEnum.Delete:
                    break;
                case ModelStateEnum.Duplicate:
                    break;
            }
        }

        public int DoSave(Entity entity,IDbConnection connection,IDbTransaction transaction)
        {
            switch (entity.GetValue<ModelStateEnum>("State"))
            {
                case ModelStateEnum.None:
                    break;
                case ModelStateEnum.Insert:
                    var id = DoInsert(entity,connection,transaction);
                    entity.SetPrimaryKey<Entity>(id);
                    return id;
                    break;
                case ModelStateEnum.Update:
                    return DoUpdate(entity, connection, transaction);
                    break;
                case ModelStateEnum.Delete:
                    DoDelete(entity.GetPrimaryKey<Entity>().ToString(),connection,transaction);
                    break;
                case ModelStateEnum.Duplicate:
                    break;
            }
            return 0;
        }

        public virtual void AfterSave(object entity, IDbConnection connection, IDbTransaction transaction)
        {

        }

        public void AfterCommitSave(Entity entity)
        {

        }
        #endregion

        public bool ValidateRole(ActionEnum action)
        {
            var hasPermision = false;
            switch (action)
            {
                case ActionEnum.Delete:
                    hasPermision = RoleConstant.MODEL_ALLOW_CUSTOMER_DELETE.Contains(typeof(Entity));
                    break;
                case ActionEnum.Insert:
                    hasPermision = RoleConstant.MODEL_ALLOW_CUSTOMER_INSERT.Contains(typeof(Entity));
                    break;
                case ActionEnum.Update:
                    hasPermision = RoleConstant.MODEL_ALLOW_CUSTOMER_UPDATE.Contains(typeof(Entity));
                    break;
                case ActionEnum.DeleteMulti:
                    hasPermision = RoleConstant.MODEL_ALLOW_CUSTOMER_DELETE_MULTI.Contains(typeof(Entity));
                    break;
            }

            return _dataBaseService.CurrentUser.IsAdmin || hasPermision;
        }

        public async Task<List<Entity>> GetByField(string fieldName, string fieldValue)
        {
            var command = $"SELECT * FROM {_tableName} WHERE {fieldName} = @{fieldName}";
            var param = new Dictionary<string, object>()
            {
                {$"@{fieldName}",fieldValue }
            };
            return await _dataBaseService.QueryUsingCommanTextAsync<Entity>(command,param);
        }

        public async Task<ServiceResponse> GetTotal(PagingPayload pagingPayLoad)
        {
            var result = new ServiceResponse();
            var whereClause = BuildWhereClause(pagingPayLoad.FilterColumns);
            var searhClause = BuidSearchClause(pagingPayLoad.Keyword);
            var procedureName = "Proc_GetTotal";
            var sortOrder = BuildSortOrder(pagingPayLoad.SortOrder);

            var param = new Dictionary<string, object>() {
                {"v_WhereClause",whereClause },
                {"v_SearchClause",searhClause },
                {"v_PageSize",pagingPayLoad.PageSize },
                {"v_PageIndex",pagingPayLoad.PageIndex },
                {"v_SortOrder", sortOrder},
                {"v_Table", _tableName}
            };

            var total = await _dataBaseService.ExecuteScalarUsingStoreProcedureAsync<int>(procedureName, param);

            result.Data = new {
                Total = total,
                CurrentPage = pagingPayLoad.PageIndex,
                TotalPage = Math.Ceiling((float)total / pagingPayLoad.PageSize)
            };

            return result;
        }

        public async Task<ServiceResponse> UpdateSingleField(InfoUpdateField updateInfo,string id)
        {
            var result = new ServiceResponse();

            var prop = typeof(Entity).GetProperty(updateInfo.FieldName);
            var attr = prop.GetCustomAttribute(typeof(AllowUpdateSingle));

            if(attr != null)
            {
                var commandUpdate = $"UPDATE {_tableName} SET {updateInfo.FieldName} = @FieldValue WHERE {_fieldPrimaryKey} = @ID";
                var param = new Dictionary<string, object>()
                {
                    {"@FieldValue",updateInfo.FieldValue },
                    {"@ID",id }
                };
                result.Success = _dataBaseService.ExecuteUsingCommandText(commandUpdate, param) > 0;
            }
            else
            {
                result.Success = false;
            }

            return result;
        }
    }
}
