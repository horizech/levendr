using System;
using System.Collections.Generic;

using Levendr.Constants;
using Levendr.Enums;
using Levendr.Models;

namespace Levendr.Exceptions
{
    public class ExceptionHandler
    {
        public static string GetExceptionErrorCode(Exception exception)
        {
            string errorCode = ErrorCode.GENERIC.ToString();
            if (exception.GetType() == typeof(LevendrErrorCodeException))
            {
                errorCode = ((LevendrErrorCodeException)exception)?.ErrorCode?.ToString() ?? exception.Message;
            }
            return errorCode;
        }
    }

}