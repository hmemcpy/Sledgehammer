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
        protected MethodInterceptor()
        {
            Cop.AsFluent();
        }

        public void InterceptMethod(MethodInfo methodInfo)
        {
            methodInfo.Override(OnInterceptMethod);
        }

        protected abstract object OnInterceptMethod(InterceptionContext context);
    }
}