using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;


using System.Text.Json.Serialization;

using Levendr.Models;
using Levendr.Enums;

namespace Levendr.Mappings
{
    public class ColumnsDefinition
    {

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        // [JsonConverter(typeof(List<JsonStringEnumConverter>))]
        [JsonPropertyName("Columns")]
        public List<ColumnInfo> Columns { get; set; }

        [JsonPropertyName("Descriptions")]
        public List<Dictionary<string, string>> Descriptions { get; set; }
    }
}
