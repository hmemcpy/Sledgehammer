using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CodeCop.Core;
using CodeCop.Core.Fluent;
using FakeItEasy;
using FakeItEasy.Configuration;
using FakeItEasy.Core;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;

namespace Sledgehammer.FakeItEasy
{
    class FakeItEasyMethodInterceptor : MethodInterceptor
    {
        protected override object OnInterceptMethod(InterceptionContext context)
        {
            var interceptedMethod = (MethodInfo)context.InterceptedMethod;

            var genericMethod = interceptedMethod.MakeGenericMethod(context.GenericArguments.ToArray());

            return Create(genericMethod.ReturnType, context);
        }

        private object Create(Type returnType, InterceptionContext context)
        {
            var target = (LambdaExpression)context.Parameters[0].Value;
            var body = (MethodCallExpression)target.Body;

            dynamic fake = new ExpandoObject();
            fake.ReturnsLazily = Return<IAfterCallSpecifiedWithOutAndRefParametersConfiguration>.Arguments<Func<IFakeObjectCall, int>>(
                func =>
                {
                    MockManager.GetManager(body.Method).ReturnValue = func(null);
                    return null;
                });
            
            //fake.Throws = Return<IAfterCallSpecifiedConfiguration>.Arguments(() => null);
            //fake.Invokes = Return<IReturnValueConfiguration<int>>.Arguments(() => null);
            //fake.MustHaveHappened = ReturnVoid.Arguments(() => { });
            //fake.CallsBaseMethod = Return<IAfterCallSpecifiedConfiguration>.Arguments(() => null);
            //fake.WhenArgumentsMatch = Return<IReturnValueConfiguration<int>>.Arguments(() => null);

            body.Method.Override(i => MockManager.GetManager(i.InterceptedMethod).ReturnValue);
            Cop.Intercept();

            return Impromptu.DynamicActLike(fake, returnType);
        }

        private static MethodInfo GetMethod(Type type, string name)
        {
            return type.GetInterfaces().Select(@interface => @interface.GetMethod(name)).FirstOrDefault(mi => mi != null);
        }

        private Func<IFakeObjectCall, int> Foo(Func<IFakeObjectCall, int> f)
        {
            return f;
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