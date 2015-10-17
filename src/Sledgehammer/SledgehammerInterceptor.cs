using System;
using System.Linq;
using System.Reflection;

namespace Sledgehammer
{
    public interface ISledgehammerContext
    {
        Type InterceptorType { get; }
        void Intercept(IMethodInterceptor interceptor);
    }

    public abstract class SledgehammerContext : ISledgehammerContext
    {
        public abstract void Intercept(IMethodInterceptor interceptor);
        public abstract Type InterceptorType { get; }

        protected static Assembly FindAssembly(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == assemblyName);
        }
    }

    public static class SledgehammerInterceptor
    {
        public static void Use<TContext>() where TContext : ISledgehammerContext
        {
            var context = Activator.CreateInstance<TContext>();
            context.Intercept((IMethodInterceptor)Activator.CreateInstance(context.InterceptorType));
        }
    }
}