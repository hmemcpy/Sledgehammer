using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sledgehammer
{
    public class MockManager
    {
        private static readonly Dictionary<MethodBase, MockManager> instances = new Dictionary<MethodBase, MockManager>();

        public static MockManager GetManager(MethodBase method)
        {
            MockManager manager;
            if (!instances.TryGetValue(method, out manager))
            {
                manager = new MockManager();
                instances[method] = manager;
            }

            return manager;
        }

        public object ReturnValue { get; set; }
        public Exception Throws { get; set; }
    }
}