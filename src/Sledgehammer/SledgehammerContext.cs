using System;
using System.Linq;
using System.Reflection;

namespace Sledgehammer
{
    public abstract class SledgehammerContext : ISledgehammerContext
    {
        public abstract void Intercept(IMethodInterceptor interceptor);
        public abstract Type InterceptorType { get; }

        protected static Assembly LoadAssembly(string assemblyName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
            if (assembly == null)
            {
                var n = new AssemblyName(assemblyName);
                assembly = Assembly.Load(n);
            }
            return assembly;
        }
    }
}