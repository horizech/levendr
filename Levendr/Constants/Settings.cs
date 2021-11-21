using System.Collections.Generic;

using Levendr.Models;

namespace Levendr.Constants
{
    public class Settings
    {

        public class Setting
        {
            public int Id { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public const string DefaultRoleOnSignup = "DefaultRoleOnSignup";
        public const string DefaultMediaSource = "DefaultMediaSource";

    }
}