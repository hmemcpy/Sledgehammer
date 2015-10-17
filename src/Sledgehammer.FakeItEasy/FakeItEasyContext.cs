using System;
using System.Linq.Expressions;
using System.Reflection;
using CodeCop.Core;

namespace Sledgehammer.FakeItEasy
{
    public sealed class FakeItEasyContext : SledgehammerContext
    {
        public override Type InterceptorType => typeof (FakeItEasyMethodInterceptor);

        public override void Intercept(IMethodInterceptor interceptor)
        {
            var fieAssembly = FindAssembly("FakeItEasy");
            if (fieAssembly == null) return;

            InterceptCallTo(interceptor, fieAssembly);
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
    }
}