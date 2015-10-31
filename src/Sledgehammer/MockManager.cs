using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sledgehammer
{
    public class MockManager
    {
        private static readonly Dictionary<MethodBase, MockManager> instances = new Dictionary<MethodBase, MockManager>();

        private Stack<ICallRule> CallRules { get; }

        private MockManager()
        {
            CallRules = new Stack<ICallRule>();
        }

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

        public void Add(ICallRule rule)
        {
            CallRules.Push(rule);
        }

        public object Execute()
        {
            var rule = CallRules.Pop();
            return rule.Execute();
        }
    }

    public interface ICallRule
    {
        object Execute();
    }

    class InvokeRule : ICallRule
    {
        private readonly Func<object> delegateToInvoke;

        public InvokeRule(Func<object> delegateToInvoke)
        {
            this.delegateToInvoke = delegateToInvoke;
        }

        public object Execute()
        {
            return delegateToInvoke();
        }
    }

    class ThrowRule : ICallRule
    {
        private readonly Exception exceptionToThrow;

        public ThrowRule(Exception exceptionToThrow)
        {
            this.exceptionToThrow = exceptionToThrow;
        }

        public object Execute()
        {
            throw exceptionToThrow;
        }
    }

    class ReturnValueRule : ICallRule
    {
        private readonly object valueToReturn;

        public ReturnValueRule(object valueToReturn)
        {
            this.valueToReturn = valueToReturn;
        }

        public object Execute()
        {
            return valueToReturn;
        }
    }
}