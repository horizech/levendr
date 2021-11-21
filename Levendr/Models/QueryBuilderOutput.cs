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
    public class QueryBuilderOutput
    {
        public string Script { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public QueryBuilderOutput() { }

        public QueryBuilderOutput(string script, Dictionary<string, object> parameters = null)
        {
            Script = script;
            Parameters = parameters;
        }

    }
}