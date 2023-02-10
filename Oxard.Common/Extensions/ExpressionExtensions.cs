using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Oxard.Common.Extensions
{
    public static class ExpressionExtensions
    {
        public static string GetPropertyName<T>(this Expression<Func<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (!(expression.Body is MemberExpression body))
                throw new ArgumentException("Invalid argument", nameof(expression));

            if (!(body.Member is PropertyInfo property))
                throw new ArgumentException("Argument is not a property", nameof(expression));

            return property.Name;
        }

        public static T GetPropertyValue<T>(this Expression<Func<T>> expression)
        {
             return expression.Compile()();
        }
    }
}