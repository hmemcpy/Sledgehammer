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
        private readonly MethodBase method;
        private static readonly Dictionary<MethodBase, MockManager> instances = new Dictionary<MethodBase, MockManager>();

        private Stack<ICallRule> CallRules { get; }

        private MockManager(MethodBase method)
        {
            this.method = method;
            CallRules = new Stack<ICallRule>();
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

        public void Add(ICallRule rule)
        {
            CallRules.Push(rule);
        }

        public object Execute()
        {
            if (CallRules.Count == 0)
            {
                Cop.Reset(method);
                return null;
            }

            var rule = CallRules.Pop();

            if (CallRules.Count == 0)
            {
                Cop.Reset(method);
            }

            return rule.Execute();
        }

        public static bool IsIntercepted<T>(Expression<Func<T>> x)
        {
            var body = (MethodCallExpression)x.Body;
            var methodInfo = body.Method;
            return methodInfo.IsIntercepted();
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