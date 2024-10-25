using System.Linq.Expressions;

namespace Blogger.Core.Helpers;

public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var param = Expression.Parameter(typeof(T), "x");

        var body1 = Expression.Invoke(expr1, param);
        var body2 = Expression.Invoke(expr2, param);

        var combined = Expression.AndAlso(body1, body2);

        return Expression.Lambda<Func<T, bool>>(combined, param);
    }
}