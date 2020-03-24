using System;
using System.Collections.Generic;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Unity.Interception.Tests
{
    public class CallCountInterceptionBehavior : IInterceptionBehavior
    {
        private int _callCount;

        [InjectionConstructor]
        public CallCountInterceptionBehavior()
        {
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            ++_callCount;
            return getNext()(input, getNext);
        }

        public int CallCount
        {
            get { return _callCount; }
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
