using System;

namespace Sledgehammer
{
    public interface ISledgehammerContext
    {
        Type InterceptorType { get; }
        void Intercept(IMethodInterceptor interceptor);
    }

    public static class Sledgehammer
    {
        public static void Use<TContext>() where TContext : ISledgehammerContext
        {
            var context = Activator.CreateInstance<TContext>();
            context.Intercept((IMethodInterceptor)Activator.CreateInstance(context.InterceptorType));
        }
    }
}