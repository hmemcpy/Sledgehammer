using System;
using System.Linq.Expressions;
using System.Reflection;
using CodeCop.Core;
using CodeCop.Core.Fluent;

namespace Sledgehammer
{
    public interface IMethodInterceptor
    {
        void InterceptMethod(MethodInfo methodInfo);
    }

    internal class MethodInterceptor : IMethodInterceptor
    {
        public MethodInterceptor()
        {
            Cop.AsFluent();
        }

        public void InterceptMethod(MethodInfo methodInfo)
        {
            methodInfo.Override(context =>
            {
                var target = (LambdaExpression) context.Parameters[0].Value;
                var body = ((MethodCallExpression) target.Body);
                body.Method.Override(c => null);
                return null;
            });

            Cop.Intercept();
        }
    }
}