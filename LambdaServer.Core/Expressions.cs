using System;
using System.Linq.Expressions;

namespace LambdaServer.Core
{
    public class Expressions
    {
        public static LambdaExpression Create(Expression<Func<object>> expression)
        {
            return expression;
        }

        public static LambdaExpression Create(Expression<Func<object, object>> expression)
        {
            return expression;
        }

        public static LambdaExpression Create(Expression<Func<object, object, object>> expression)
        {
            return expression;
        }
    }
}
