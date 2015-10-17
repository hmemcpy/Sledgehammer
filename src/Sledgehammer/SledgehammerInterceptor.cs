using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using CodeCop.Core;
using CodeCop.Core.Contracts;
using CodeCop.Core.Fluent;
using Sledgehammer;

public interface ISledgehammerContext
{
    Type InterceptorType { get; }
    void Intercept(IMethodInterceptor interceptor);
}

public static class SledgehammerInterceptor
{
    public static void Use<TContext>() where TContext : ISledgehammerContext
    {
        var context = Activator.CreateInstance<TContext>();
        context.Intercept((IMethodInterceptor)Activator.CreateInstance(context.InterceptorType));
    }

    public static bool IsIntercepted<T>(Expression<Func<T>> expr)
    {
        var method = ((MethodCallExpression)expr.Body).Method;

        return method.IsIntercepted();
    }
}