﻿using System;
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

namespace Levendr.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly ILogger<PermissionsController> _logger;

        public PermissionsController(ILogger<PermissionsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetPermissions")]
        public async Task<APIResult> GetPermissions()
        {
            return await ServiceManager.Instance.GetService<PermissionsService>().GetPermissions();

        }

        [HttpPost("AddPermission")]
        public async Task<APIResult> AddPermission(Dictionary<string, object> data)
        {
            try{
                if (data == null || data.Count() == 0 || !data.ContainsKey("Name") || !data.ContainsKey("Description"))
                {
                    return APIResult.GetSimpleFailureResult("Permission must contain Name and Description!");
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
                        APIResult result = await ServiceManager.Instance.GetService<PermissionsService>().AddPermission(data);
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

        [HttpPut("UpdatePermission")]
        public async Task<APIResult> UpdatePermission(string name, Dictionary<string, object> data)
        {
            try{
                if (data == null || data.Count() == 0 || !data.ContainsKey("Name") || !data.ContainsKey("Description"))
                {
                    return APIResult.GetSimpleFailureResult("Permission must contain Name and Description!");
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
                        APIResult result = await ServiceManager.Instance.GetService<PermissionsService>().UpdatePermission(name, data);
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

        [HttpDelete("DeletePermission")]
        public async Task<APIResult> DeletePermission(string name)
        {
            try{
                List<string> permissions = Permissions.GetUserPermissions(User);
                if (permissions.Contains("CanDeleteTablesData"))
                {
                    try
                    {
                        APIResult result = await ServiceManager.Instance.GetService<PermissionsService>().DeletePermission(name);
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
