using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sledgehammer;
using Sledgehammer.FakeItEasy;

public sealed class FakeItEasyContext : ISledgehammerContext
{
    public Type InterceptorType => typeof (FakeItEasyMethodInterceptor);

    public void Intercept(IMethodInterceptor interceptor)
    {
        var fieAssembly = FindFakeItEasyAssembly();
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
    }

    private void InterceptReturns(IMethodInterceptor interceptor, Assembly assembly)
    {
        var retValConfigType = assembly.GetType("FakeItEasy.Configuration.IReturnValueArgumentValidationConfiguration`1");

        var returnMethods = retValConfigType.GetMethods().Where(info => info.Name == "ReturnsLazily");
        foreach (MethodInfo returnMethod in returnMethods)
        {
        }
    }

    private Assembly FindFakeItEasyAssembly()
    {
        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == "FakeItEasy");
    }
}