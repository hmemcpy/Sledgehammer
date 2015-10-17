using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CodeCop.Core;
using CodeCop.Core.Fluent;

namespace Sledgehammer
{
    public interface IMethodInterceptor
    {
        void InterceptMethod(MethodInfo methodInfo);
    }

    public abstract class MethodInterceptor : IMethodInterceptor
    {
        public MethodInterceptor()
        {
            Cop.AsFluent();
        }

        public void InterceptMethod(MethodInfo methodInfo)
        {
            methodInfo.Override(OnInterceptMethod);

            Cop.Intercept();
        }

        protected abstract object OnInterceptMethod(InterceptionContext context);
    }
}