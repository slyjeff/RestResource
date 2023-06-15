using System.Linq.Expressions;
using System.Reflection;

namespace Slysoft.RestResource.Utils; 

internal static class MapActionExtensions {
    public static string? Evaluate<T>(this Expression<Func<T, object>> mapAction) {
        var expression = mapAction.Body;

        var unaryExpression = expression as UnaryExpression;
        if ((unaryExpression?.Operand ?? expression) is not MemberExpression memberExpression) {
            return null;
        }

        var property = memberExpression.Member as PropertyInfo;
        return property?.Name;
    }
}