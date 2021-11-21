using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;


using Levendr.Models;
using Levendr.Enums;

namespace Levendr.Mappings
{
    public class TableDefinition
    {

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("AddAdditionalColumns")]
        public bool AddAdditionalColumns { get; set; }

        [JsonPropertyName("Columns")]
        public List<ColumnInfo> Columns { get; set; }

        [JsonPropertyName("DefaultRows")]
        public List<Dictionary<string, object>> DefaultRows { get; set; }

        [JsonPropertyName("IncludeTables")]
        public List<TableIncluded> IncludeTables { get; set; }

    }
}
