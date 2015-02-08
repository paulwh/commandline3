using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CommandLine.Helpers {
    public static class ReflectionHelper {
        public static MethodInfo GetGenericMethodDefinition<T>(Expression<Action<T>> action) {
            if (!(action.Body is MethodCallExpression)) {
                throw new ArgumentException(
                    "The lamda expression must contain a method call at it's root",
                    "action"
                );
            } else {
                return ((MethodCallExpression)action.Body).Method.GetGenericMethodDefinition();
            }
        }

        public static MethodInfo GetGenericMethodDefinition(Expression<Action> action) {
            if (!(action.Body is MethodCallExpression)) {
                throw new ArgumentException(
                    "The lamda expression must contain a method call at it's root",
                    "action"
                );
            } else {
                return ((MethodCallExpression)action.Body).Method.GetGenericMethodDefinition();
            }
        }
    }
}
