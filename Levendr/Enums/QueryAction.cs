using System;
using System.Collections.Generic;

namespace Levendr.Enums
{
    public enum QueryAction
    {
        Null,
        CreateDatabase,
        CreateSchema,
        CreateTable,
        SelectRows,
        SelectCount,
        InsertRows,
        UpdateRows,
        DeleteRows,
        Custom
    }

}