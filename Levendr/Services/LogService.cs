using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Configuration;

using Levendr.Databases;
using Levendr.Enums;
using Levendr.Constants;

namespace Levendr.Services
{
    public class LogService : BaseService
    {

        public LogService(IConfiguration configuration) : base(configuration)
        {
        }

        private LoggingLevel loggingLevel { get; set; }

        public LoggingLevel GetLoggingLevel()
        {
            return loggingLevel;
        }

        public void SetLoggingLevel(LoggingLevel loggingLevel)
        {
            this.loggingLevel = loggingLevel;
        }

        public void SetLoggingLevelFromEnvironment()
        {
            this.loggingLevel = (LoggingLevel)Enum.Parse(
                typeof(LoggingLevel),
                (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.LoggingLevel, Config.DefaultLoggingLevel)
            );
        }

        public void Print(object text, LoggingLevel loggingLevel)
        {
            if ((int)GetLoggingLevel() >= (int)loggingLevel)
            {
                Console.WriteLine(text.ToString());
            }
        }

    }

}