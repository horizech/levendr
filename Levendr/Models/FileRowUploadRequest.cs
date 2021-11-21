using System;
using System.Web;
using Microsoft.AspNetCore.Http;

using System.Collections.Generic;

using Levendr.Services;
using Levendr.Enums;
using Levendr.Models;

namespace Levendr.Models
{
    public class FileRowUploadRequest
    {
        public IFormFile File { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }

}