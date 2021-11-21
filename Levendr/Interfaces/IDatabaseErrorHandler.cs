using System;
using System.Collections.Generic;
using System.Text.Json;

using Levendr.Enums;
using Levendr.Models;

namespace Levendr.Interfaces
{
    public interface IDatabaseErrorHandler
    {
        ErrorCode GetErrorCode(string errorMessage);
        ErrorCode GetErrorCode(Exception errorException);
        void ThrowLevendrException(string errorMessage);
        void ThrowLevendrException(Exception errorException);
        void ThrowLevendrException(ErrorCode errorCode, string errorMessage);
        void ThrowLevendrExceptionWithCustomMessage(string errorMessage);
    }
}