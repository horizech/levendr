using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using Levendr.Services;
using Levendr.Models;
using Levendr.Helpers;
using Levendr.Constants;

namespace Levendr.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class LevendrController : ControllerBase
    {
        private readonly ILogger<LevendrController> _logger;

        public LevendrController(ILogger<LevendrController> logger)
        {
            _logger = logger;
        }

        [HttpGet("IsInitialized")]
        public async Task<APIResult> IsInitialized()
        {
            return await ServiceManager.Instance.GetService<LevendrService>().IsInitialized();
        }

        [HttpPost("Initialize")]
        public async Task<APIResult> Initialize(LoginRequest loginDetails)
        {
            if (loginDetails == null)
            {
                return APIResult.GetSimpleFailureResult("Credentials not valid!");
            }
            if (loginDetails.Username == null || loginDetails.Username.Length < 6)
            {
                return APIResult.GetSimpleFailureResult("Username should be at least 6 characters!");
            }

            if (loginDetails.Password == null || loginDetails.Password.Length == 0)
            {
                return APIResult.GetSimpleFailureResult("password is not valid!");
            }

            return await ServiceManager.Instance.GetService<LevendrService>().Initialize(loginDetails.Username, loginDetails.Password);
        }

        [HttpGet("GetTablesList")]
        [Authorize]
        public async Task<APIResult> GetTablesList()
        {
            List<string> permissions = Permissions.GetUserPermissions(User);
            if (permissions.Contains("CanReadTables"))
            {
                return await ServiceManager.Instance.GetService<TableService>().GetTablesList(Schemas.Levendr);
            }
            else
            {
                return APIResult.GetSimpleFailureResult("Not allowed to create tables!");
            }
        }

        [HttpGet("GetTableColumns")]
        [Authorize]
        public async Task<APIResult> GetTableColumns(string table)
        {
            List<string> permissions = Permissions.GetUserPermissions(User);
            if (permissions.Contains("CanReadTables"))
            {
                return await ServiceManager.Instance.GetService<TableService>().GetTableColumns(Schemas.Levendr, table);
            }
            else
            {
                return APIResult.GetSimpleFailureResult("Not allowed to create tables!");
            }
        }


        


    }
}
