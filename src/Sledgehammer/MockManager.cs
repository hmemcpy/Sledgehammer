using System.Collections.Generic;
using System.Reflection;

namespace Sledgehammer
{
    public class MockManager
    {
        private static readonly Dictionary<MethodBase, MockManager> instances = new Dictionary<MethodBase, MockManager>();

        private readonly MethodBase method;

        private MockManager(MethodBase method)
        {
            this.method = method;
        }

        public static MockManager GetManager(MethodBase method)
        {
            MockManager manager;
            if (!instances.TryGetValue(method, out manager))
            {
                manager = new MockManager(method);
                instances[method] = manager;
            }

            return manager;
        }

        public object ReturnValue { get; set; }
    }
}