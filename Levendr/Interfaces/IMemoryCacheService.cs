using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

using Levendr.Models;
using Levendr.Mappings;
using Levendr.Enums;
using Levendr.Helpers;
using Levendr.Services;

namespace Levendr.Interfaces
{
    public interface IMemoryCacheService
    {
        Task<APIResult> Get(string key);

        void Set(string key, APIResult entry, MemoryCacheEntryOptions options = null);

        void Remove(string key);
    }
}