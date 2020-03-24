using System;
using System.Collections.Generic;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Unity.Interception.Tests
{
    public class DelegateInterceptionBehavior : IInterceptionBehavior
    {
        public static readonly Func<IEnumerable<Type>> NoRequiredInterfaces = () => Type.EmptyTypes;

        private readonly Func<IMethodInvocation, GetNextInterceptionBehaviorDelegate, IMethodReturn> _invoke;
        private readonly Func<IEnumerable<Type>> _requiredInterfaces;

        public DelegateInterceptionBehavior(Func<IMethodInvocation, GetNextInterceptionBehaviorDelegate, IMethodReturn> invoke)
            : this(invoke, NoRequiredInterfaces)
        { }

        public DelegateInterceptionBehavior(
            Func<IMethodInvocation, GetNextInterceptionBehaviorDelegate, IMethodReturn> invoke,
            Func<IEnumerable<Type>> requiredInterfaces)
        {
            _invoke = invoke;
            _requiredInterfaces = requiredInterfaces;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            return _invoke(input, getNext);
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return _requiredInterfaces();
        }

        /// <summary>
        /// Returns a flag indicating if this behavior will actually do anything when invoked.
        /// </summary>
        /// <remarks>This is used to optimize interception. If the behaviors won't actually
        /// do anything (for example, PIAB where no policies match) then the interception
        /// mechanism can be skipped completely.</remarks>
        public bool WillExecute
        {
            get { return true; }
        }
    }
}
