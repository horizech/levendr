using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Levendr.Models;

namespace Levendr.Models
{
    public class SignupRequest
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? Role {get; set;}
    }
}