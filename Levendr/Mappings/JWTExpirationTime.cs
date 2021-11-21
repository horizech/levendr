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
    public class JWTExpirationTime
    {

        [JsonPropertyName("Value")]
        public int Value { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("TimeType")]
        public JWTExpirationTimeTypes TimeType { get; set; }

    }
}
