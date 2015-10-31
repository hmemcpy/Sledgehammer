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
            var fieAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == assemblyName);
            if (fieAssembly == null)
            {
                var n = new AssemblyName(assemblyName);
                fieAssembly = Assembly.Load(n);
            }
            return fieAssembly;
        }
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