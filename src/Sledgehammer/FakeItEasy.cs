using System;
using System.Linq.Expressions;
using System.Reflection;
using CodeCop.Core;
using Sledgehammer.Interceptors;

namespace Sledgehammer
{
    public sealed class FakeItEasy : SledgehammerContext
    {
        public override Type InterceptorType => typeof(FakeItEasyMethodInterceptor);

        public override void Intercept(IMethodInterceptor interceptor)
        {
            var fieAssembly = FindAssembly("FakeItEasy");
            if (fieAssembly == null) return;

            InterceptCallTo(interceptor, fieAssembly);
        }

        private static void InterceptCallTo(IMethodInterceptor interceptor, Assembly assembly)
        {
            var aType = assembly.GetType("FakeItEasy.A");

            var callToOfTFunc = GetCallToOfT(aType, typeof(Func<>));
            var callToOfTAction = GetCallToOfT(aType, typeof(Action));

            interceptor.InterceptMethod(callToOfTFunc);
            interceptor.InterceptMethod(callToOfTAction);

            Cop.Intercept();
        }

        private static MethodInfo GetCallToOfT(Type aType, Type genericArg)
        {
            var unboundExpression = typeof(Expression<>);
            var boundExpr = unboundExpression.MakeGenericType(genericArg);
            var isGeneric = genericArg.IsGenericTypeDefinition;
            return isGeneric ? aType.GetGenericMethod("CallTo", new[] { boundExpr }) : aType.GetMethod("CallTo", new []{ boundExpr });
        }
    }
}