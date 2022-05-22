using System;
using System.Collections.Generic;
using Levendr.Models;

namespace Levendr.Constants
{
    public class Config
    {
        // Database
        public const string DatabaseType = "LEVENDR_DATABASE_TYPE";
        public const string DatabaseHost = "LEVENDR_DATABASE_HOST";
        public const string DatabasePort = "LEVENDR_DATABASE_PORT";
        public const string DatabaseName = "LEVENDR_DATABASE_NAME";
        public const string DatabaseUsername = "LEVENDR_DATABASE_USERNAME";
        public const string DatabasePassword = "LEVENDR_DATABASE_PASSWORD";

        // Plugins
        public const string AWSSESS3Bucket = "LEVENDR_AWS_SES_S3_BUCKET";
        public const string AWSSESS3Region = "LEVENDR_AWS_SES_S3_REGION";
        public const string AWSSESS3AccessKeyId = "LEVENDR_AWS_SES_S3_ACCESS_KEY_ID";
        public const string AWSSESS3SecretAccessKey = "LEVENDR_AWS_SES_S3_SECRET_ACCESS_KEY";
        public const string AWSSESS3SessionToken = "LEVENDR_AWS_SES_S3_SESSION_TOKEN";
        
        // JWT Token
        public const string JwtTokenIssuer = "LEVENDR_JWT_TOKEN_ISSUER";
        public const string JwtTokenSecretKey = "LEVENDR_JWT_TOKEN_SECRET_KEY";
        public const string DefaultJwtTokenSecretKey = "KEY__SECRET__KEY";
        public const string DefaultJwtTokenIssuer = "Levendr";

        // Logging
        public const string LoggingLevel = "LEVENDR_LOGGING_LEVEL";
        public const string DefaultLoggingLevel = "Errors";

        // Media
        public const string MediaLocalPath = "LEVENDR_MEDIA_LOCAL_PATH";
        public const string MediaS3Url = "LEVENDR_MEDIA_S3_URL";
        public const string MediaS3Path = "LEVENDR_MEDIA_S3_PATH";
        public const string MediaS3AccessKeyId = "LEVENDR_MEDIA_S3_ACCESS_KEY_ID";
        public const string MediaS3SecretAccessKey = "LEVENDR_MEDIA_S3_SECRET_ACCESS_KEY";

        public const string ConfigurationDirectory = @"Configurations";
        public const string TablesJson = "Tables.json";

    }
}