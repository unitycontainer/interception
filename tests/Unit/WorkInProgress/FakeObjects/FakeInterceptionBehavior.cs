

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    internal class FakeInterceptionBehavior : IInterceptionBehavior
    {
        public Func<IMethodInvocation, GetNextInterceptionBehaviorDelegate, IMethodReturn> InvokeFunc { get; set; }

        IMethodReturn IInterceptionBehavior.Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            return this.InvokeFunc.Invoke(input, getNext);
        }

        IEnumerable<Type> IInterceptionBehavior.GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        bool IInterceptionBehavior.WillExecute
        {
            get { return true; }
        }
    }
}
