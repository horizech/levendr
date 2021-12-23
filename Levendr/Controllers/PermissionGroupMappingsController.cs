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
using Levendr.Filters;
using System.Security.Claims;

namespace Levendr.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class PermissionGroupMappingsController : ControllerBase
    {
        private readonly ILogger<PermissionGroupMappingsController> _logger;

        public PermissionGroupMappingsController(ILogger<PermissionGroupMappingsController> logger)
        {
            _logger = logger;
        }

        [LevendrAuthorized]
        [HttpGet("GetPermissionGroupMappings")]
        public async Task<APIResult> GetPermissionGroupMappings()
        {
            return await ServiceManager.Instance.GetService<PermissionGroupMappingsService>().GetPermissionGroupMappings();

        }

        [LevendrAuthorized]
        [HttpPost("AddPermissionGroupMapping")]
        public async Task<APIResult> AddPermissionGroupMapping(Dictionary<string, object> data)
        {
            try{
                if (data == null || data.Count() == 0 || !data.ContainsKey("PermissionGroup") || !data.ContainsKey("Permission") || !data.ContainsKey("IsSystem"))
                {
                    return APIResult.GetSimpleFailureResult("PermissionGroupMapping must contain PermissionGroup, Permission and IsSystem!");
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

                try
                {
                    APIResult result = await ServiceManager.Instance.GetService<PermissionGroupMappingsService>().AddPermissionGroupMapping(data);
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
            catch(LevendrErrorCodeException e) {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
            catch(Exception e) {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
        }

        //[LevendrClaimAuthorized(ClaimTypes.Authentication, "CanUpdateTablesData")]
        [LevendrAuthorized]
        [HttpPut("UpdatePermissionGroupMapping")]
        public async Task<APIResult> UpdatePermissionGroupMapping(int Id, Dictionary<string, object> data)
        {
            try{
                if (data == null || data.Count() == 0 || !data.ContainsKey("PermissionGroup") || !data.ContainsKey("Permission") || !data.ContainsKey("IsSystem"))
                {
                    return APIResult.GetSimpleFailureResult("PermissionGroupMapping must contain PermissionGroup, Permission and IsSystem!");
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

                try
                {
                    APIResult result = await ServiceManager.Instance.GetService<PermissionGroupMappingsService>().UpdatePermissionGroupMapping(Id, data);
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
            catch(LevendrErrorCodeException e) {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
            catch(Exception e) {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
        }

        [LevendrAuthorized]
        [HttpDelete("DeletePermissionGroupMapping")]
        public async Task<APIResult> DeletePermissionGroupMapping(int Id)
        {
            try{
               try
                {
                    APIResult result = await ServiceManager.Instance.GetService<PermissionGroupMappingsService>().DeletePermissionGroupMapping(Id);
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
            catch(LevendrErrorCodeException e) {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
            catch(Exception e) {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
        }
    }
}
