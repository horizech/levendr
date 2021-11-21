using System;
using System.Collections.Generic;

namespace Levendr.Interfaces
{
    public interface IDatabaseConnection
    {

        bool SetDatabaseConnectionUsingEnvironment();

        string GetDatabaseConnectionString();

    }
}