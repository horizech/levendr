using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Levendr.Models;
using Levendr.Enums;
using Levendr.Helpers;

namespace Levendr.Interfaces
{
    public interface IQueryBuilder
    {
        QueryBuilderOutput Build(LevendrQuery query, int level = 0, Dictionary<string, object> parameters = null);
    }
}