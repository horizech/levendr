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
using System.Security.Claims;

namespace Levendr.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class PermissionGroupsController : ControllerBase
    {
        private readonly ILogger<PermissionGroupsController> _logger;

        public PermissionGroupsController(ILogger<PermissionGroupsController> logger)
        {
            _logger = logger;
        }

        [LevendrAuthorized]
        [HttpGet("GetPermissionGroups")]
        public async Task<APIResult> GetPermissionGroups()
        {
            return await ServiceManager.Instance.GetService<PermissionGroupsService>().GetPermissionGroups();

        }

        [LevendrAuthorized]
        [HttpPost("AddPermissionGroup")]
        public async Task<APIResult> AddPermissionGroup(Dictionary<string, object> data)
        {
            try{
                if (data == null || data.Count() == 0 || !data.ContainsKey("Name") || !data.ContainsKey("Description") || !data.ContainsKey("IsSystem"))
                {
                    return APIResult.GetSimpleFailureResult("PermissionGroup must contain Name, Description and IsSystem!");
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
                        APIResult result = await ServiceManager.Instance.GetService<PermissionGroupsService>().AddPermissionGroup(data);
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

        //[LevendrClaimAuthorized(ClaimTypes.Authentication, "CanUpdateTablesData")]
        [LevendrAuthorized]
        [HttpPut("UpdatePermissionGroup")]
        public async Task<APIResult> UpdatePermissionGroup(string name, Dictionary<string, object> data)
        {
            try{
                if (data == null || data.Count() == 0 || !data.ContainsKey("Name") || !data.ContainsKey("Description") || !data.ContainsKey("IsSystem"))
                {
                    return APIResult.GetSimpleFailureResult("PermissionGroup must contain Name, Description and IsSystem!");
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
                        APIResult result = await ServiceManager.Instance.GetService<PermissionGroupsService>().UpdatePermissionGroup(name, data);
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
        [HttpDelete("DeletePermissionGroup")]
        public async Task<APIResult> DeletePermissionGroup(string name)
        {
            try{
                List<string> permissions = Permissions.GetUserPermissions(User);
                if (permissions.Contains("CanDeleteTablesData"))
                {
                    try
                    {
                        APIResult result = await ServiceManager.Instance.GetService<PermissionGroupsService>().DeletePermissionGroup(name);
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
