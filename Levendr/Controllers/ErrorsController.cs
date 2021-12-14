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
    public class ErrorsController : ControllerBase
    {
        private readonly ILogger<ErrorsController> _logger;

        public ErrorsController(ILogger<ErrorsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Error")]
        public APIResult Error(string errorCode, string errorMessage)
        {
            return new APIResult {
                Success = false,
                Data = null,
                ErrorCode = "AUTH001",
                Message = errorMessage
            };
        }
    }
}
