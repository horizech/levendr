using System;
using System.Collections.Generic;

using Levendr.Constants;
using Levendr.Enums;
using Levendr.Models;

namespace Levendr.Exceptions
{
    public class LevendrErrorCodeException : Exception
    {
        public ErrorCode? ErrorCode {get; set;}

        public LevendrErrorCodeException() { }
        public LevendrErrorCodeException(ErrorCode errorCode) : base(errorCode.ToString()) { 
            ErrorCode = errorCode;
        }

        public LevendrErrorCodeException(ErrorCode errorCode, string message) : base(message) {
            ErrorCode = errorCode;
        }

        public LevendrErrorCodeException(string message) : base(message) { }
        public LevendrErrorCodeException(string message, System.Exception inner) : base(message, inner) { }
        protected LevendrErrorCodeException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }

}