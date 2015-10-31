using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using CodeCop.Core;
using CodeCop.Core.Fluent;
using FakeItEasy;
using FakeItEasy.Configuration;
using FakeItEasy.Core;
using ImpromptuInterface;

namespace Sledgehammer
{
    class FakeItEasyMethodInterceptor : MethodInterceptor
    {
        protected override object OnInterceptMethod(InterceptionContext context)
        {
            var interceptedMethod = (MethodInfo)context.InterceptedMethod;

            Type returnType;
            if (context.GenericArguments.Count > 0)
            {
                var genericMethod = interceptedMethod.MakeGenericMethod(context.GenericArguments.ToArray());
                returnType = genericMethod.ReturnType;
            }
            else
            {
                returnType = interceptedMethod.ReturnType;
            }

            return Create(returnType, context);
        }

        private static object Create(Type returnType, InterceptionContext context)
        {
            var target = (LambdaExpression)context.Parameters[0].Value;
            var body = (MethodCallExpression)target.Body;

            dynamic fake = new ExpandoObject();

            fake.ReturnsLazily = (Func<dynamic, dynamic>)(f =>
            {
                MockManager.GetManager(body.Method).Add(new ReturnValueRule(f(null)));
                return null;
            });
            fake.Throws = (Func<dynamic, Exception>)(f =>
            {
                MockManager.GetManager(body.Method).Add(new ThrowRule(f(null)));
                return null;
            });
            fake.Invokes = (Func<Delegate, dynamic>)(a =>
            {
                MockManager.GetManager(body.Method).Add(new InvokeRule(() => a.FastDynamicInvoke(new object[] { null })));
                return null;
            });
            //fake.MustHaveHappened = ReturnVoid.Arguments(() => { });
            //fake.CallsBaseMethod = Return<IAfterCallSpecifiedConfiguration>.Arguments(() => null);
            //fake.WhenArgumentsMatch = Return<IReturnValueConfiguration<int>>.Arguments(() => null);

            body.Method.Override(i =>
            {
                var mockManager = MockManager.GetManager(i.InterceptedMethod);
                return mockManager.Execute();
            });

            Cop.Intercept();

            return Impromptu.DynamicActLike(fake, returnType);
        }

        public class X<T> : IReturnValueArgumentValidationConfiguration<T>
        {
            public IAfterCallSpecifiedWithOutAndRefParametersConfiguration ReturnsLazily(Func<IFakeObjectCall, T> valueProducer)
            {
                throw new NotImplementedException();
            }

            public IAfterCallSpecifiedConfiguration Throws(Func<IFakeObjectCall, Exception> exceptionFactory)
            {
                throw new NotImplementedException();
            }

            public IReturnValueConfiguration<T> Invokes(Action<IFakeObjectCall> action)
            {
                throw new NotImplementedException();
            }

            public void MustHaveHappened(Repeated repeatConstraint)
            {
                throw new NotImplementedException();
            }

            public IAfterCallSpecifiedConfiguration CallsBaseMethod()
            {
                throw new NotImplementedException();
            }

            public IReturnValueConfiguration<T> WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
            {
                throw new NotImplementedException();
            }
        }
    }
}