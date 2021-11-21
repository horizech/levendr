using System;
using System.Collections.Generic;

using Levendr.Services;
using Levendr.Enums;
using Levendr.Models;

namespace Levendr.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

}