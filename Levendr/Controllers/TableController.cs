using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using Levendr.Services;
using Levendr.Models;
using Levendr.Enums;
using Levendr.Constants;
using Levendr.Helpers;
using Levendr.Exceptions;
using Levendr.Interfaces;

namespace Levendr.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class TableController : ControllerBase
    {
        private readonly ILogger<TableController> _logger;

        public TableController(ILogger<TableController> logger)
        {
            _logger = logger;
        }

        [HttpPost("CreateTable")]
        [Authorize]
        public async Task<APIResult> CreateTable(string table, List<ColumnInfo> columns)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            
            bool UsingPredefinedColumns = false;
            List<string> predefinedColumns = Columns.PredefinedColumns.Descriptions.Select(x => x["Name"].ToLower()).ToList();

            if((columns?.Count ?? 0) > 0)
            {
                columns.ForEach(x =>
                {
                    string name = x.Name.ToLower();
                    if (predefinedColumns.Contains(name))
                    {
                        UsingPredefinedColumns = true;
                    }
                });

                if (UsingPredefinedColumns)
                {
                    return APIResult.GetSimpleFailureResult("Using predefined column(s)!");
                }
            }

            List<string> permissions = Permissions.GetUserPermissions(User);
            if (permissions.Contains("CanCreateTables"))
            {
                return await ServiceManager.Instance.GetService<TableService>().CreateTable(Schemas.Application, table, columns);
            }
            else
            {
                return APIResult.GetSimpleFailureResult("Not allowed to create tables!");
            }
        }

        [HttpGet("GetTablesList")]
        [Authorize]
        public async Task<APIResult> GetTablesList()
        {
            return await ServiceManager.Instance.GetService<TableService>().GetTablesList(Schemas.Application);
        }

        [HttpGet("GetTableColumns")]
        [Authorize]
        public async Task<APIResult> GetTableColumns(string table)
        {
            return await ServiceManager.Instance.GetService<TableService>().GetTableColumns(Schemas.Application, table);
        }


        [HttpGet("GetPredefinedColumns")]
        [Authorize]
        public APIResult GetPredefinedColumns()
        {
            return ServiceManager.Instance.GetService<TableService>().GetPredefinedColumns();
        }

        [HttpPost("AddColumn")]
        public async Task<APIResult> AddColumn(string table, ColumnInfo columnInfo)
        {
            try{
                if (table == null || table.Count() == 0)
                {
                    return APIResult.GetSimpleFailureResult("Table is not valid!");
                }
                if (columnInfo == null || string.IsNullOrEmpty(columnInfo.Name) || columnInfo.Datatype < 0)
                {
                    return APIResult.GetSimpleFailureResult("Column information is not valid!");
                }

                List<string> predefinedColumns = Columns.PredefinedColumns.Descriptions.Select(x => x["Name"].ToLower()).ToList();

                if(predefinedColumns.Contains(columnInfo.Name))
                {
                    return APIResult.GetSimpleFailureResult("Cannot delete predefined column!");
                }
                
                List<string> permissions = Permissions.GetUserPermissions(User);
                if (permissions.Contains("CanCreateTables"))
                {
                    Task<APIResult> createTask = ServiceManager.Instance.GetService<TableService>().AddColumn(Schemas.Application, table, columnInfo);
                    try
                    {
                        APIResult result = await createTask;
                        return result; 
                    }
                    catch (Exception e)
                    {
                        IDatabaseErrorHandler handler = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseErrorHandler();
                        ErrorCode errorCode = handler.GetErrorCode(e.Message);
                        if(errorCode == ErrorCode.DB520) {
                            // It's a null value column constraint violation
                            return APIResult.GetSimpleFailureResult(errorCode.GetMessage() + ": " + e.Message.Split('\"')[1]);
                        }
                        else {
                            return APIResult.GetSimpleFailureResult(e.Message);
                        }
                    }

                
                }
                else
                {
                    return APIResult.GetSimpleFailureResult("Not allowed to delete column!");
                }
            }
            catch(LevendrErrorCodeException e) {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
            catch(Exception e) {
                return APIResult.GetSimpleFailureResult(e.Message);
            }

        }
        
        [HttpDelete("DeleteColumn")]
        public async Task<APIResult> DeleteColumn(string table, string column)
        {
            try{
                if (table == null || table.Count() == 0)
                {
                    return APIResult.GetSimpleFailureResult("Table is not valid!");
                }
                if (column == null || column.Count() == 0)
                {
                    return APIResult.GetSimpleFailureResult("Data is not valid!");
                }

                List<string> predefinedColumns = Columns.PredefinedColumns.Descriptions.Select(x => x["Name"].ToLower()).ToList();

                if(predefinedColumns.Contains(column))
                {
                    return APIResult.GetSimpleFailureResult("Cannot delete predefined column!");
                }
                
                List<string> permissions = Permissions.GetUserPermissions(User);
                if (permissions.Contains("CanCreateTables"))
                {
                    Task<APIResult> createTask = ServiceManager.Instance.GetService<TableService>().DeleteColumn(Schemas.Application, table, column);
                    try
                    {
                        APIResult result = await createTask;
                        return result; 
                    }
                    catch (Exception e)
                    {
                        IDatabaseErrorHandler handler = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseErrorHandler();
                        ErrorCode errorCode = handler.GetErrorCode(e.Message);
                        if(errorCode == ErrorCode.DB520) {
                            // It's a null value column constraint violation
                            return APIResult.GetSimpleFailureResult(errorCode.GetMessage() + ": " + e.Message.Split('\"')[1]);
                        }
                        else {
                            return APIResult.GetSimpleFailureResult(e.Message);
                        }
                    }

                
                }
                else
                {
                    return APIResult.GetSimpleFailureResult("Not allowed to delete column!");
                }
            }
            catch(LevendrErrorCodeException e) {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
            catch(Exception e) {
                return APIResult.GetSimpleFailureResult(e.Message);
            }

        }
        
        [HttpPost("InsertRows")]
        public async Task<APIResult> InsertRows(string table, List<Dictionary<string, object>> data)
        {
            try{
                if (table == null || table.Count() == 0)
                {
                    return APIResult.GetSimpleFailureResult("Table is not valid!");
                }
                if (data == null || data.Count() == 0)
                {
                    return APIResult.GetSimpleFailureResult("Data is not valid!");
                }

                List<string> predefinedColumns = Columns.PredefinedColumns.Descriptions.Select(x => x["Name"].ToLower()).ToList();

                for (int i = 0; i < data.Count; i++)
                {
                    data[i].Keys.ToList().ForEach(key =>
                    {
                        if (predefinedColumns.Contains(key.ToLower()))
                        {
                            ServiceManager.Instance.GetService<LogService>().Print(string.Format("Removing key: {0}", key), LoggingLevel.Info);
                            data[i].Remove(key);
                        }
                    });
                }

                for (int i = 0; i < data.Count; i++)
                {
                    data[i].Keys.ToList().ForEach(key =>
                    {
                        if (predefinedColumns.Contains(key.ToLower()))
                        {
                            ServiceManager.Instance.GetService<LogService>().Print(string.Format("Removing key: {0}", key), LoggingLevel.Info);
                            data[i].Remove(key);
                        }
                    });

                    Columns.AppendCreatedInfo(data[i], Users.GetUserId(User));

                }

                List<string> permissions = Permissions.GetUserPermissions(User);
                if (permissions.Contains("CanCreateTablesData") || permissions.Contains("CanCreateTableData" + table))
                {
                    Task<APIResult> insertTask = ServiceManager.Instance.GetService<TableService>().InsertRows(Schemas.Application, table, data);
                    try
                    {
                        APIResult result = await insertTask;
                        return result; 
                    }
                    catch (Exception e)
                    {
                        IDatabaseErrorHandler handler = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseErrorHandler();
                        ErrorCode errorCode = handler.GetErrorCode(e.Message);
                        if(errorCode == ErrorCode.DB520) {
                            // It's a null value column constraint violation
                            return APIResult.GetSimpleFailureResult(errorCode.GetMessage() + ": " + e.Message.Split('\"')[1]);
                        }
                        else {
                            return APIResult.GetSimpleFailureResult(e.Message);
                        }
                    }

                
                }
                else if (permissions.Contains("CanCreateOwnData"))
                {
                    return await ServiceManager.Instance.GetService<TableService>().InsertRows(Schemas.Application, table, data);
                }
                else
                {
                    return APIResult.GetSimpleFailureResult("Not allowed to write data!");
                }
            }
            catch(LevendrErrorCodeException e) {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
            catch(Exception e) {
                return APIResult.GetSimpleFailureResult(e.Message);
            }

        }

        [HttpGet("GetRows")]
        [Authorize]
        public async Task<APIResult> GetRows(string table)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            List<string> permissions = Permissions.GetUserPermissions(User);
            if (permissions.Contains("CanReadTablesData") || permissions.Contains("CanReadTableData" + table))
            {
                return await ServiceManager.Instance.GetService<TableService>().GetRows(Schemas.Application, table);
            }
            else if (permissions.Contains("CanReadOwnData"))
            {
                List<QuerySearchItem> UserParameters = new List<QuerySearchItem>{
                    new QuerySearchItem(){
                        Name = "CreatedBy",
                        CaseSensitive = false,
                        Condition = Enums.ColumnCondition.Equal,
                        Value = Users.GetUserId(User)
                    }
                };
                return await ServiceManager.Instance.GetService<TableService>().GetRowsByConditions(Schemas.Application, table, UserParameters);
            }
            else
            {
                return APIResult.GetSimpleFailureResult("Not allowed to read data!");
            }
        }

        [HttpPost("GetRowsByConditions")]
        public async Task<APIResult> GetRowsByConditions(string table, List<QuerySearchItem> parameters)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            List<string> permissions = Permissions.GetUserPermissions(User);
            if (permissions.Contains("CanReadTablesData") || permissions.Contains("CanReadTableData" + table))
            {
                return await ServiceManager.Instance.GetService<TableService>().GetRowsByConditions(Schemas.Application, table, parameters);
            }
            else if (permissions.Contains("CanReadOwnData"))
            {
                bool doesUserParamExist = false;

                for (int i = 0; i < parameters.Count; i++)
                {
                    if (parameters[i].Name.ToLower().Equals("createdby"))
                    {
                        parameters[i].Name = "CreatedBy";
                        parameters[i].Value = Users.GetUserId(User);
                        parameters[i].CaseSensitive = false;
                        parameters[i].Condition = Enums.ColumnCondition.Equal;
                        doesUserParamExist = true;
                    }
                }

                if (!doesUserParamExist)
                {
                    parameters.Add(
                        new QuerySearchItem()
                        {
                            Name = "CreatedBy",
                            CaseSensitive = false,
                            Condition = Enums.ColumnCondition.Equal,
                            Value = Users.GetUserId(User)
                        }
                    );
                }
                return await ServiceManager.Instance.GetService<TableService>().GetRowsByConditions(Schemas.Application, table, parameters);
            }
            else
            {
                return APIResult.GetSimpleFailureResult("Not allowed to read data!");
            }
        }

        [HttpPut("UpdateRows")]
        public async Task<APIResult> UpdateRows(string table, UpdateRequest request)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }
            if (request == null || request.Data == null || request.Data.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Data is not valid!");
            }

            List<string> predefinedColumns = Columns.PredefinedColumns.Descriptions.Select(x => x["Name"].ToLower()).ToList();

            request.Data.Keys.ToList().ForEach(key =>
            {
                if (predefinedColumns.Contains(key.ToLower()))
                {
                    ServiceManager.Instance.GetService<LogService>().Print(string.Format("Removing key: {0}", key), LoggingLevel.Info);
                    request.Data.Remove(key);
                }
            });

            Columns.AppendUpdatedInfo(request.Data, Users.GetUserId(User));

            if (request.Parameters == null)
            {
                request.Parameters = new List<QuerySearchItem>();
            }

            List<string> permissions = Permissions.GetUserPermissions(User);
            if (permissions.Contains("CanUpdateTablesData") || permissions.Contains("CanUpdateTableData" + table))
            {
                return await ServiceManager.Instance.GetService<TableService>().UpdateRows(Schemas.Application, table, request.Data, request.Parameters);
            }
            else if (permissions.Contains("CanUpdateOwnData"))
            {


                bool doesUserParamExist = false;

                for (int i = 0; i < request.Parameters.Count; i++)
                {
                    if (request.Parameters[i].Name.ToLower().Equals("createdby"))
                    {
                        request.Parameters[i].Name = "CreatedBy";
                        request.Parameters[i].Value = Users.GetUserId(User);
                        request.Parameters[i].CaseSensitive = false;
                        request.Parameters[i].Condition = Enums.ColumnCondition.Equal;
                        doesUserParamExist = true;
                    }
                }

                if (!doesUserParamExist)
                {
                    request.Parameters.Add(
                        new QuerySearchItem()
                        {
                            Name = "CreatedBy",
                            CaseSensitive = false,
                            Condition = Enums.ColumnCondition.Equal,
                            Value = Users.GetUserId(User)
                        }
                    );
                }

                return await ServiceManager.Instance.GetService<TableService>().UpdateRows(Schemas.Application, table, request.Data, request.Parameters);
            }
            else
            {
                return APIResult.GetSimpleFailureResult("Not allowed to update data!");
            }
        }


        [HttpDelete("DeleteRows")]
        public async Task<APIResult> DeleteRows(string table, List<QuerySearchItem> parameters)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            List<string> permissions = Permissions.GetUserPermissions(User);
            if (permissions.Contains("CanDeleteTablesData") || permissions.Contains("CanUpdateTableData" + table))
            {
                return await ServiceManager.Instance.GetService<TableService>().DeleteRows(Schemas.Application, table, parameters);
            }
            else if (permissions.Contains("CanDeleteOwnData"))
            {
                bool doesUserParamExist = false;

                for (int i = 0; i < parameters.Count; i++)
                {
                    if (parameters[i].Name.ToLower().Equals("createdby"))
                    {
                        parameters[i].Name = "CreatedBy";
                        parameters[i].Value = Users.GetUserId(User);
                        parameters[i].CaseSensitive = false;
                        parameters[i].Condition = Enums.ColumnCondition.Equal;
                        doesUserParamExist = true;
                    }
                }

                if (!doesUserParamExist)
                {
                    parameters.Add(
                        new QuerySearchItem()
                        {
                            Name = "CreatedBy",
                            CaseSensitive = false,
                            Condition = Enums.ColumnCondition.Equal,
                            Value = Users.GetUserId(User)
                        }
                    );
                }
                return await ServiceManager.Instance.GetService<TableService>().DeleteRows(Schemas.Application, table, parameters);
            }
            else
            {
                return APIResult.GetSimpleFailureResult("Not allowed to delete data!");
            }
        }
    }
}
