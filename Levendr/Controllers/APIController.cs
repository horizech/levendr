using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using Levendr.Services;
using Levendr.Models;
using Levendr.Enums;
using Levendr.Constants;
using Levendr.Helpers;
using Levendr.Filters;

namespace Levendr.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class APIController : ControllerBase
    {
        private readonly ILogger<APIController> _logger;

        public APIController(ILogger<APIController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public APIResult Get()
        {
            return new APIResult()
            {
                Success = true,
                Message = "Server up and running"
            };
        }

#nullable enable
        [LevendrAuthorized]
        [HttpGet("{table}")]
        [Authorize]
        public async Task<APIResult> GetRows(string table, [FromQuery] int? limit, [FromQuery] int? offset, [FromQuery] string? orderBy, [FromQuery] string? orderDescendingBy, [FromQuery] string? groupBy)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            SelectSettings selectSettings = new SelectSettings
            {
                Limit = limit ?? -1,
                Offset = offset ?? -1,
                OrderBy = orderBy,
                OrderDescendingBy = orderDescendingBy,
                GroupBy = groupBy
            };

            // If user only data access
            // List<QuerySearchItem> UserParameters = new List<QuerySearchItem>{
            //     new QuerySearchItem(){
            //         Name = "CreatedBy",
            //         CaseSensitive = false,
            //         Condition = Enums.ColumnCondition.Equal,
            //         Value = Users.GetUserId(User)
            //     }
            // };
            // return await ServiceManager.Instance.GetService<APIService>().GetRowsByConditions(Schemas.Application, table, UserParameters, selectSettings);
            
            return await ServiceManager.Instance.GetService<APIService>().GetRows(Schemas.Application, table, selectSettings);            
        }
#nullable disable

        [LevendrAuthorized]
        [HttpGet("{table}/{id}")]
        [Authorize]
        public async Task<APIResult> GetRow(string table, int id)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            if (id < 1)
            {
                return APIResult.GetSimpleFailureResult("Id is not valid!");
            }

            // If user only data access 
            // List<QuerySearchItem> UserParameters = new List<QuerySearchItem>{
            //     new QuerySearchItem(){
            //         Name = "CreatedBy",
            //         CaseSensitive = false,
            //         Condition = Enums.ColumnCondition.Equal,
            //         Value = Users.GetUserId(User)
            //     },
            //     new QuerySearchItem(){
            //         Name = "Id",
            //         CaseSensitive = false,
            //         Condition = Enums.ColumnCondition.Equal,
            //         Value = id
            //     }
            // };
            // return await ServiceManager.Instance.GetService<APIService>().GetRowsByConditions(Schemas.Application, table, UserParameters);
            
            List<QuerySearchItem> UserParameters = new List<QuerySearchItem>{
                new QuerySearchItem(){
                    Name = "Id",
                    CaseSensitive = false,
                    Condition = Enums.ColumnCondition.Equal,
                    Value = id
                }
            };
            
            
            return await ServiceManager.Instance.GetService<APIService>().GetRowsByConditions(Schemas.Application, table, UserParameters);            
        }

        [LevendrAuthorized]
        [HttpPost("{table}")]
        public async Task<APIResult> InsertRow(string table, Dictionary<string, object> data)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }
            if (data == null || data.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Data is not valid!");
            }

            List<string> predefinedColumns = Columns.PredefinedColumns.Descriptions.Select(x => x["Name"].ToLower()).ToList();

            data.Keys.ToList().ForEach(key =>
            {
                if (predefinedColumns.Contains(key.ToLower()))
                {
                    ServiceManager.Instance.GetService<LogService>().Print(string.Format("Removing key: {0}", key), LoggingLevel.Info);
                    data.Remove(key);
                }
            });

            Columns.AppendCreatedInfo(data, Users.GetUserId(User));

            return await ServiceManager.Instance.GetService<APIService>().InsertRow(Schemas.Application, table, data);
        }

        [LevendrAuthorized]
        [HttpPut("{table}/{id}")]
        public async Task<APIResult> UpdateRow(string table, int id, Dictionary<string, object> data)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            if (data == null || data.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Data is not valid!");
            }

            UpdateRequest request = new UpdateRequest()
            {
                Parameters = new List<QuerySearchItem>{
                    new QuerySearchItem(){
                        Name = "Id",
                        CaseSensitive = false,
                        Condition = Enums.ColumnCondition.Equal,
                        Value = id
                    }
                },
                Data = data
            };

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

            // If user only data access
            // request.Parameters.Add(
            //     new QuerySearchItem()
            //     {
            //         Name = "CreatedBy",
            //         CaseSensitive = false,
            //         Condition = Enums.ColumnCondition.Equal,
            //         Value = Users.GetUserId(User)
            //     }
            // );

            // return await ServiceManager.Instance.GetService<APIService>().UpdateRow(Schemas.Application, table, request.Data, request.Parameters);
            
            return await ServiceManager.Instance.GetService<APIService>().UpdateRow(Schemas.Application, table, request.Data, request.Parameters);
        }


        [LevendrAuthorized]
        [HttpDelete("{table}/{id}")]
        public async Task<APIResult> DeleteRow(string table, int id)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            if (id < 1)
            {
                return APIResult.GetSimpleFailureResult("Id is not valid!");
            }

            List<QuerySearchItem> parameters = new List<QuerySearchItem>(){
                new QuerySearchItem()
                    {
                        Name = "Id",
                        CaseSensitive = false,
                        Condition = Enums.ColumnCondition.Equal,
                        Value = id
                    }
            };

            // if user only data access
            // parameters.Add(
            //     new QuerySearchItem()
            //     {
            //         Name = "CreatedBy",
            //         CaseSensitive = false,
            //         Condition = Enums.ColumnCondition.Equal,
            //         Value = Users.GetUserId(User)
            //     }
            // );

            // return await ServiceManager.Instance.GetService<APIService>().DeleteRow(Schemas.Application, table, parameters);
            
            return await ServiceManager.Instance.GetService<APIService>().DeleteRow(Schemas.Application, table, parameters);            
        }
    }
}
