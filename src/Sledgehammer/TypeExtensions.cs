using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sledgehammer
{
    public static class TypeExtensions
    {
        private class SimpleTypeComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type x, Type y)
            {
                return x.Assembly == y.Assembly &&
                    x.Namespace == y.Namespace &&
                    x.Name == y.Name;
            }

            public int GetHashCode(Type obj)
            {
                throw new NotImplementedException();
            }
        }

        public static MethodInfo GetGenericMethodDefinition(this Type type, string name, int arguments)
        {
            var methods = type.GetMethods();
            return methods.Where(m => m.Name == name)
                          .Where(method => method.IsGenericMethod)
                          .FirstOrDefault(method => method.GetGenericArguments().Length == arguments);
        }

        public static MethodInfo GetGenericMethod(this Type type, string name, Type[] parameterTypes)
        {
            var methods = type.GetMethods();
            foreach (var method in methods.Where(m => m.Name == name))
            {
                if (!method.IsGenericMethodDefinition) continue;
                
                var methodParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

                if (methodParameterTypes.SequenceEqual(parameterTypes, new SimpleTypeComparer()))
                {
                    return method;
                }
            }

            return null;
        }
    }
}