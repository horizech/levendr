using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;

using Levendr.Models;
using Levendr.Exceptions;
using Levendr.Helpers;
using Levendr.Services;
using Levendr.Enums;
using Levendr.Interfaces;

namespace Levendr.Interfaces
{
    public interface IQueryExecuter
    {
        Task<IEnumerable<T>> Execute<T>(QueryBuilderOutput queryBuilderOutput);
        Task<IEnumerable<dynamic>> Execute(QueryBuilderOutput queryBuilderOutput);
        Task<bool> ExecuteNonQuery(QueryBuilderOutput queryBuilderOutput);
    }
}