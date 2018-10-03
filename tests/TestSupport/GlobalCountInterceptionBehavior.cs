using System;
using System.Collections.Generic;
using Unity;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Microsoft.Practices.Unity.TestSupport
{
    public class GlobalCountInterceptionBehavior : IInterceptionBehavior
    {
        public static Dictionary<string, int> Calls = new Dictionary<string, int>();
        private string callHandlerName;

        [InjectionConstructor]
        public GlobalCountInterceptionBehavior()
            : this("default")
        {
        }

        public GlobalCountInterceptionBehavior(string callHandlerName)
        {
            this.callHandlerName = callHandlerName;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (!Calls.ContainsKey(callHandlerName))
            {
                Calls.Add(callHandlerName, 0);
            }
            Calls[callHandlerName]++;

            return getNext().Invoke(input, getNext);
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public bool WillExecute { get { return true; } }
    }
}
