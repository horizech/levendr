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
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [LevendrAuthorized]
        [HttpGet("GetUserRole/{userId}")]
        public async Task<APIResult> GetUserRole(int userId)
        {
            if (userId < 1)
            {
                return APIResult.GetSimpleFailureResult("User Id is not vaild!");
            }

            return await ServiceManager.Instance.GetService<UsersService>().GetUserRole(userId);
        }

        [LevendrAuthorized]
        [HttpGet("GetUserPermissions/{userId}")]
        public async Task<APIResult> GetUserPermissions(int userId)
        {
            if (userId < 1)
            {
                return APIResult.GetSimpleFailureResult("User Id is not vaild!");
            }

            return await ServiceManager.Instance.GetService<UsersService>().GetUserPermissions(userId);
        }
    
        [LevendrAuthorized]
        [HttpGet("GetUser/{Id}")]
        public async Task<APIResult> GetUser(int Id)
        {
            return await ServiceManager.Instance.GetService<UsersService>().GetUser(Id);

        }

        [LevendrAuthorized]
        [HttpGet("GetUsers")]
        public async Task<APIResult> GetUsers()
        {
            return await ServiceManager.Instance.GetService<UsersService>().GetUsers();

        }

        [LevendrAuthorized]
        [HttpPost("AddUser")]
        public async Task<APIResult> AddUser(Dictionary<string, object> data)
        {
            try{
                if (data == null || data.Count() == 0 || !data.ContainsKey("Username") || !data.ContainsKey("Password"))
                {
                    return APIResult.GetSimpleFailureResult("User must contain Username and Password!");
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
                    APIResult result = await ServiceManager.Instance.GetService<UsersService>().AddUser(data);
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
        [HttpPut("UpdateUser")]
        public async Task<APIResult> UpdateUser(int Id, Dictionary<string, object> data)
        {
            try{
                if (data == null || data.Count() == 0)
                {
                    return APIResult.GetSimpleFailureResult("Nothing to update!");
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

                data.Add("LastUpdatedOn", DateTime.UtcNow);

                try
                {
                    APIResult result = await ServiceManager.Instance.GetService<UsersService>().UpdateUser(Id, data);
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
        [HttpDelete("DeleteUser")]
        public async Task<APIResult> DeleteUser(int Id)
        {
            try{
                try
                {
                    APIResult result = await ServiceManager.Instance.GetService<UsersService>().DeleteUser(Id);
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
