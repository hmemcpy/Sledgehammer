using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using CodeCop.Core;
using CodeCop.Core.Fluent;

namespace Sledgehammer
{
    public class MockManager
    {
        private static readonly Dictionary<MethodBase, MockManager> instances = new Dictionary<MethodBase, MockManager>();

        private Stack<ICallRule> CallRules { get; }
        public int Invocations { get; private set; }

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
            var countingRule = new CountingRule(rule, this);
            CallRules.Push(countingRule);
        }

        public object Execute()
        {
            var rule = CallRules.Count == 1 ? CallRules.Peek() : CallRules.Pop();

            return rule.Execute();
        }

        internal static bool IsIntercepted<T>(Expression<Func<T>> x)
        {
            var body = (MethodCallExpression)x.Body;
            var methodInfo = body.Method;
            return methodInfo.IsIntercepted();
        }

        private class CountingRule : ICallRule
        {
            private readonly ICallRule rule;
            private readonly MockManager manager;

            public CountingRule(ICallRule rule, MockManager manager)
            {
                this.rule = rule;
                this.manager = manager;
            }

            public object Execute()
            {
                manager.Invocations++;
                return rule.Execute();
            }
        }

        public static void Reset()
        {
            instances.Clear();
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