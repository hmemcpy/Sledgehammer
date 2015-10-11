using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sledgehammer;

public sealed class FakeItEasyContext : ISledgehammerContext
{
    public void Intercept(IMethodInterceptor interceptor)
    {
        var fieAssembly = FindFakeItEasyAssembly();
        if (fieAssembly == null) return;

        var aType = fieAssembly.GetType("FakeItEasy.A");

        var unboundExpression = typeof(Expression<>);
        var unboundFunc = typeof(Func<>);
        var boundExprOfFunc = unboundExpression.MakeGenericType(unboundFunc);

        var callToOfT = aType.GetGenericMethod("CallTo", new [] { boundExprOfFunc });

        interceptor.InterceptMethod(callToOfT);
    }

    private Assembly FindFakeItEasyAssembly()
    {
        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == "FakeItEasy");
    }
}