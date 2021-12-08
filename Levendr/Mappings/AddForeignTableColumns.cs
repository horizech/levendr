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
    public class AddForeignTableColumns
    {
        [JsonPropertyName("ForeignName")]
        public string ForeignName { get; set; }
        
        [JsonPropertyName("OutputName")]
        public string OutputName { get; set; }        
    }
}
