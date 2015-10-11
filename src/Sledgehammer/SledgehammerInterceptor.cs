using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using CodeCop.Core;
using CodeCop.Core.Contracts;
using Sledgehammer;

public interface ISledgehammerContext
{
    void Intercept(IMethodInterceptor interceptor);
}

public static class SledgehammerInterceptor
{
    public static void Use<TContext>() where TContext : ISledgehammerContext
    {
        var context = Activator.CreateInstance<TContext>();
        context.Intercept(new MethodInterceptor());
    }

    public static bool IsIntercepted<T>(Expression<Func<T>> expr)
    {
        // todo hack
        var fluentBag = typeof(Cop).Assembly.GetType("CodeCop.FluentExtensions.FluentBag");
        var getBag = fluentBag.GetMethod("GetBag", BindingFlags.Static | BindingFlags.NonPublic);
        var bag = (List<Tuple<MethodBase, ICopIntercept>>)getBag.Invoke(null, null);

        var method = ((MethodCallExpression) expr.Body).Method;

        return bag.Exists(tuple => tuple.Item1 == method);
    }
}