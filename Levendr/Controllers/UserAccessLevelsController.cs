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

namespace Levendr.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class UserAccessLevelsController : ControllerBase
    {
        private readonly ILogger<UserAccessLevelsController> _logger;

        public UserAccessLevelsController(ILogger<UserAccessLevelsController> logger)
        {
            _logger = logger;
        }

        [LevendrAuthorized]
        [HttpGet("GetUserAccessLevels")]
        public async Task<APIResult> GetUserAccessLevels()
        {
            return await ServiceManager.Instance.GetService<UserAccessLevelsService>().GetUserAccessLevels();

        }

        [LevendrAuthorized]
        [HttpPost("AddUserAccessLevel")]
        public async Task<APIResult> AddUserAccessLevel(Dictionary<string, object> data)
        {
            try{
                if (data == null || data.Count() == 0 || !data.ContainsKey("Name") || !data.ContainsKey("Description"))
                {
                    return APIResult.GetSimpleFailureResult("UserAccessLevel must contain Name and Description!");
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
                    APIResult result = await ServiceManager.Instance.GetService<UserAccessLevelsService>().AddUserAccessLevel(data);
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
        [HttpPut("UpdateUserAccessLevel")]
        public async Task<APIResult> UpdateUserAccessLevel(string name, Dictionary<string, object> data)
        {
            try{
                if (data == null || data.Count() == 0 || !data.ContainsKey("Name") || !data.ContainsKey("Description"))
                {
                    return APIResult.GetSimpleFailureResult("UserAccessLevel must contain Name and Description!");
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
                    APIResult result = await ServiceManager.Instance.GetService<UserAccessLevelsService>().UpdateUserAccessLevel(name, data);
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
        [HttpDelete("DeleteUserAccessLevel")]
        public async Task<APIResult> DeleteUserAccessLevel(string name)
        {
            try{
                try
                {
                    APIResult result = await ServiceManager.Instance.GetService<UserAccessLevelsService>().DeleteUserAccessLevel(name);
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
