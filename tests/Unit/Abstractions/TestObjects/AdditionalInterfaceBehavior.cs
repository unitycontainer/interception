using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Unity.Interception.Tests
{
    public class AdditionalInterfaceBehavior : IInterceptionBehavior
    {
        private static readonly MethodInfo DoNothingMethod = typeof(IAdditionalInterface).GetMethod(nameof(IAdditionalInterface.DoNothing));
        private readonly bool _isImplicit = true;

        public AdditionalInterfaceBehavior()
        {
            _isImplicit = true;
        }

        public AdditionalInterfaceBehavior(bool implicitlyAddInterface)
        {
            _isImplicit = implicitlyAddInterface;
        }

        /// <summary>
        /// Implement this method to execute your behavior processing.
        /// </summary>
        /// <param name="input">Inputs to the current call to the target.</param>
        /// <param name="getNext">Delegate to execute to get the next delegate in the behavior chain.</param>
        /// <returns>Return value from the target.</returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (input.MethodBase == DoNothingMethod)
            {
                return ExecuteDoNothing(input);
            }
            return getNext()(input, getNext);
        }

        private IMethodReturn ExecuteDoNothing(IMethodInvocation input)
        {
            IMethodReturn returnValue = input.CreateMethodReturn(10);
            return returnValue;
        }

        /// <summary>
        /// Returns the interfaces required by the behavior for the objects it intercepts.
        /// </summary>
        /// <returns>The required interfaces.</returns>
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            if (_isImplicit)
            {
                return new[] { typeof(IAdditionalInterface) };
            }
            return Type.EmptyTypes;
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
