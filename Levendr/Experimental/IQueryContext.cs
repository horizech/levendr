using System.Linq.Expressions;

namespace Levendr.Experimental
{
    public interface IQueryContext
    {
        object Execute(Expression expression, bool isEnumerable);
    }
}
