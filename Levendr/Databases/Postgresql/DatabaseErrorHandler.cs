using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Dapper;
using Npgsql;

using Levendr.Services;
using Levendr.Models;
using Levendr.Interfaces;
using Levendr.Enums;
using Levendr.Helpers;
using Levendr.Constants;
using Levendr.Exceptions;


namespace Levendr.Databases.Postgresql
{
    public class DatabaseErrorHandler : IDatabaseErrorHandler
    {
        public DatabaseErrorHandler()
        {
        }

        public ErrorCode GetErrorCode(string errorMessage)
        {
            if (errorMessage.Contains("42P01") || errorMessage.Contains("DB510"))
            {
                return ErrorCode.DB510;
            }
            if (errorMessage.Contains("23502") || errorMessage.Contains("DB520"))
            {
                return ErrorCode.DB520;
            }
            if (errorMessage.Contains("42703") || errorMessage.Contains("DB521"))
            {
                return ErrorCode.DB521;
            }
            return ErrorCode.DB004;
        }

        public ErrorCode GetErrorCode(Exception errorException)
        {
            return GetErrorCode(errorException.Message);
        }

        public void ThrowLevendrException(string errorMessage)
        {
            throw new LevendrErrorCodeException(GetErrorCode(errorMessage));
        }

        public void ThrowLevendrException(Exception errorException)
        {
            ThrowLevendrException(errorException.Message);
        }

        public void ThrowLevendrException(ErrorCode errorCode, string errorMessage)
        {
            throw new LevendrErrorCodeException(errorCode, errorMessage);
        }

        public void ThrowLevendrExceptionWithCustomMessage(string errorMessage)
        {
            throw new LevendrErrorCodeException(errorMessage);
        }

        

    }
}