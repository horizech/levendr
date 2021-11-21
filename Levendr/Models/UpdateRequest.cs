using System;
using System.Collections.Generic;

using Levendr.Services;
using Levendr.Models;
using Levendr.Interfaces;
using Levendr.Enums;

namespace Levendr.Models
{
    public class UpdateRequest
    {
        public Dictionary<string, object> Data { get; set; }
        public List<QuerySearchItem> Parameters { get; set; }

    }
}