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
    public class SettingsController : ControllerBase
    {
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(ILogger<SettingsController> logger)
        {
            _logger = logger;
        }

        [LevendrAuthorized]
        [HttpGet("GetSetting/{key}")]
        public async Task<APIResult> GetSetting(string key)
        {
            return await ServiceManager.Instance.GetService<SettingsService>().GetSetting(key);

        }

        [LevendrAuthorized]
        [HttpGet("GetSettings")]
        public async Task<APIResult> GetSettings()
        {
            return await ServiceManager.Instance.GetService<SettingsService>().GetSettings();

        }

        [LevendrAuthorized]
        [HttpPost("AddSetting")]
        public async Task<APIResult> AddSetting(Dictionary<string, object> data)
        {
            try{
                if (data == null || data.Count() == 0 || !data.ContainsKey("Key") || !data.ContainsKey("Value"))
                {
                    return APIResult.GetSimpleFailureResult("Setting must contain Key and Value!");
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
                    APIResult result = await ServiceManager.Instance.GetService<SettingsService>().AddSetting(data);
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
        [HttpPut("UpdateSetting")]
        public async Task<APIResult> UpdateSetting(string key, Dictionary<string, object> data)
        {
            try{
                if (data == null || data.Count() == 0 || !data.ContainsKey("Key") || !data.ContainsKey("Value"))
                {
                    return APIResult.GetSimpleFailureResult("Setting must contain Key and Value!");
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
                    APIResult result = await ServiceManager.Instance.GetService<SettingsService>().UpdateSetting(key, data);
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
        [HttpDelete("DeleteSetting")]
        public async Task<APIResult> DeleteSetting(string key)
        {
            try{
                try
                {
                    APIResult result = await ServiceManager.Instance.GetService<SettingsService>().DeleteSetting(key);
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
