using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Levendr.Models;
using Levendr.Mappings;
using Levendr.Helpers;
using Levendr.Services;
using Levendr.Enums;
using Levendr.Interfaces;

namespace Levendr.Models
{
    public class LevendrQuery
    {
        public QueryAction Action { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string CustomQuery { get; set; }
        public Dictionary<string, object> CustomParameters { get; set; }
        public List<Dictionary<string, object>> Rows { get; set; }
        public List<QuerySearchItem> Conditions { get; set; }
        public List<ColumnInfo> ColumnsDefinitions { get; set; }
        public Dictionary<string, string> SelectedOutputColumns { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string OrderBy { get; set; }
        public bool IsOrderDescending { get; set; }
        public string GroupBy { get; set; }
        public LevendrQuery() { }

    }
}