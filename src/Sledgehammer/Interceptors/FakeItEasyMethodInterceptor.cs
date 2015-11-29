using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CodeCop.Core;
using CodeCop.Core.Extensions;
using CodeCop.Core.Fluent;
using ImpromptuInterface;

namespace Sledgehammer.Interceptors
{
    class FakeItEasyMethodInterceptor : MethodInterceptor
    {
        protected override object OnInterceptMethod(InterceptionContext context)
        {
            var interceptedMethod = (MethodInfo)context.InterceptedMethod;
            var target = (LambdaExpression)context.Parameters[0].Value;
            var body = (MethodCallExpression)target.Body;

            if (!body.Method.IsStatic)
            {
                var method = interceptedMethod.IsGenericMethod
                    ? interceptedMethod.MakeGenericMethod(context.GenericArguments.ToArray())
                    : interceptedMethod;

                return method.Execute(context.Sender, context.Parameters.Select(p => p.Value).ToArray());
            }

            Type returnType;
            if (interceptedMethod.IsGenericMethod)
            {
                var genericMethod = interceptedMethod.MakeGenericMethod(context.GenericArguments.ToArray());
                returnType = genericMethod.ReturnType;
            }
            else
            {
                returnType = interceptedMethod.ReturnType;
            }

            return Create(body.Method, returnType);
        }

        private static object Create(MethodBase targetMethod, Type returnType)
        {
            dynamic fake = new ExpandoObject();

            fake.ReturnsLazily = (Func<dynamic, dynamic>)(f =>
            {
                MockManager.GetManager(targetMethod).Add(new ReturnValueRule(f(null)));
                return null;
            });
            fake.Throws = (Func<dynamic, Exception>)(f =>
            {
                MockManager.GetManager(targetMethod).Add(new ThrowRule(f(null)));
                return null;
            });
            fake.Invokes = (Func<Delegate, dynamic>)(a =>
            {
                MockManager.GetManager(targetMethod).Add(new InvokeRule(() => a.FastDynamicInvoke(new object[] { null })));
                return null;
            });
            //fake.MustHaveHappened = ReturnVoid.Arguments(() => { });
            //fake.CallsBaseMethod = Return<IAfterCallSpecifiedConfiguration>.Arguments(() => null);
            //fake.WhenArgumentsMatch = Return<IReturnValueConfiguration<int>>.Arguments(() => null);

            targetMethod.Override(i =>
            {
                var mockManager = MockManager.GetManager(i.InterceptedMethod);
                return mockManager.Execute();
            });

            Cop.Intercept();

            return Impromptu.DynamicActLike(fake, returnType);
        }

        //public class X<T> : IReturnValueArgumentValidationConfiguration<T>
        //{
        //    public IAfterCallSpecifiedWithOutAndRefParametersConfiguration ReturnsLazily(Func<IFakeObjectCall, T> valueProducer)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public IAfterCallSpecifiedConfiguration Throws(Func<IFakeObjectCall, Exception> exceptionFactory)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public IReturnValueConfiguration<T> Invokes(Action<IFakeObjectCall> action)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public void MustHaveHappened(Repeated repeatConstraint)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public IAfterCallSpecifiedConfiguration CallsBaseMethod()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public IReturnValueConfiguration<T> WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
    }
}