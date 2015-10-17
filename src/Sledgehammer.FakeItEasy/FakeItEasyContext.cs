using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CodeCop.Core;
using Sledgehammer;
using Sledgehammer.FakeItEasy;

public sealed class FakeItEasyContext : SledgehammerContext
{
    public override Type InterceptorType => typeof (FakeItEasyMethodInterceptor);

    public override void Intercept(IMethodInterceptor interceptor)
    {
        var fieAssembly = FindAssembly("FakeItEasy");
        if (fieAssembly == null) return;

        InterceptCallTo(interceptor, fieAssembly);
        InterceptReturns(interceptor, fieAssembly);
    }

    private static void InterceptCallTo(IMethodInterceptor interceptor, Assembly assembly)
    {
        var aType = assembly.GetType("FakeItEasy.A");

        var unboundExpression = typeof (Expression<>);
        var unboundFunc = typeof (Func<>);
        var boundExprOfFunc = unboundExpression.MakeGenericType(unboundFunc);

        var callToOfT = aType.GetGenericMethod("CallTo", new[] {boundExprOfFunc});

        interceptor.InterceptMethod(callToOfT);

        Cop.Intercept();
    }

    private void InterceptReturns(IMethodInterceptor interceptor, Assembly assembly)
    {
        var retValConfigType = assembly.GetType("FakeItEasy.Configuration.IReturnValueArgumentValidationConfiguration`1");

        var returnMethods = retValConfigType.GetMethods().Where(info => info.Name == "ReturnsLazily");
        foreach (MethodInfo returnMethod in returnMethods)
        {
        }
    }
}