using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;


using Levendr.Models;
using Levendr.Enums;

namespace Levendr.Mappings
{
    public class ColumnsInfo
    {
        [JsonPropertyName("Info")]
        public Dictionary<string, string> Columns { get; set; }
    }
}
