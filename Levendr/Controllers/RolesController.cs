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
using Levendr.Interfaces;
using Levendr.Exceptions;
using Levendr.Filters;

namespace Levendr.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly ILogger<RolesController> _logger;

        public RolesController(ILogger<RolesController> logger)
        {
            _logger = logger;
        }

        [LevendrAuthorized]
        [HttpGet("GetRoles")]
        public async Task<APIResult> GetRoles()
        {
            return await ServiceManager.Instance.GetService<RolesService>().GetRoles();

        }

        [LevendrAuthorized]
        [HttpPost("AddRole")]
        public async Task<APIResult> AddRole(Dictionary<string, object> data)
        {
            try{
                if (data == null || data.Count() == 0 || !data.ContainsKey("Name") || !data.ContainsKey("Description") || !data.ContainsKey("Level"))
                {
                    return APIResult.GetSimpleFailureResult("Role must contain Name, Description and Level!");
                }

                List<string> predefinedColumns = Columns.PredefinedColumns.Descriptions.Select(x => x["Name"].ToLower()).ToList();

                for (int i = 0; i < data.Count; i++)
                {
                    data.Keys.ToList().ForEach(key =>
                    {
                        if (predefinedColumns.Contains(key.ToLower()))
                        {
                            ServiceManager.Instance.GetService<LogService>().Print(string.Format("Removing key: {0}", key), LoggingLevel.Info);
                            data.Remove(key);
                        }
                    });
                }

                Columns.AppendCreatedInfo(data, Users.GetUserId(User));

                List<string> permissions = Permissions.GetUserPermissions(User);
                if (permissions.Contains("CanCreateTablesData"))
                {                    
                    try
                    {
                        APIResult result = await ServiceManager.Instance.GetService<RolesService>().AddRole(data);
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

        [LevendrAuthorized]
        [HttpPut("UpdateRole")]
        public async Task<APIResult> UpdateRole(string name, Dictionary<string, object> data)
        {
            try{
                if (data == null || data.Count() == 0 || !data.ContainsKey("Name") || !data.ContainsKey("Description") || !data.ContainsKey("Level"))
                {
                    return APIResult.GetSimpleFailureResult("Role must contain Name, Description and Level!");
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

                Columns.AppendUpdatedInfo(data, Users.GetUserId(User));

                List<string> permissions = Permissions.GetUserPermissions(User);
                if (permissions.Contains("CanUpdateTablesData"))
                {                    
                    try
                    {
                        APIResult result = await ServiceManager.Instance.GetService<RolesService>().UpdateRole(name, data);
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

        [LevendrAuthorized]
        [HttpDelete("DeleteRole")]
        public async Task<APIResult> DeleteRole(string name)
        {
            try{
                List<string> permissions = Permissions.GetUserPermissions(User);
                if (permissions.Contains("CanDeleteTablesData"))
                {
                    try
                    {
                        APIResult result = await ServiceManager.Instance.GetService<RolesService>().DeleteRole(name);
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
                    return APIResult.GetSimpleFailureResult("Not allowed to delete data!");
                }
            }
            catch(LevendrErrorCodeException e) {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
            catch(Exception e) {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
        }
    }
}
