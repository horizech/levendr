using System;
using System.Collections.Generic;

using Levendr.Services;
using Levendr.Models;
using Levendr.Interfaces;
using Levendr.Enums;

namespace Levendr.Models
{
    public class QuerySearchItem
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ColumnCondition Condition { get; set; }
        public bool CaseSensitive { get; set; }

        public void Print()
        {
            ServiceManager.Instance.GetService<LogService>().Print(string.Format("Name: {0}, Value: {1}, Condition: {2}, CaseSensitive: {3}", Name, Value.ToString(), Condition.ToString(), CaseSensitive.ToString()), LoggingLevel.All);
        }
    }

}