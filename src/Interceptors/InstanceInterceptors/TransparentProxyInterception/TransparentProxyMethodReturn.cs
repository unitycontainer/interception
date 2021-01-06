using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Permissions;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Unity.Interception.Interceptors.InstanceInterceptors.TransparentProxyInterception
{
    /// <summary>
    /// An implementation of <see cref="IMethodReturn"/> that wraps the
    /// remoting call and return messages.
    /// </summary>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
    internal class TransparentProxyMethodReturn : IMethodReturn
    {
        private readonly IMethodCallMessage _callMessage;
        private readonly ParameterCollection _outputs;
        private readonly IDictionary<string, object> _invocationContext;
        private readonly object[] _arguments;
        private object _returnValue;
        private Exception _exception;

        /// <summary>
        /// Creates a new <see cref="TransparentProxyMethodReturn"/> object that contains a
        /// return value.
        /// </summary>
        /// <param name="callMessage">The original call message that invoked the method.</param>
        /// <param name="returnValue">Return value from the method.</param>
        /// <param name="arguments">Collections of arguments passed to the method (including the new
        /// values of any out params).</param>
        /// <param name="invocationContext">Invocation context dictionary passed into the call.</param>
        public TransparentProxyMethodReturn(IMethodCallMessage callMessage, object returnValue, object[] arguments, IDictionary<string, object> invocationContext)
        {
            _callMessage = callMessage;
            _invocationContext = invocationContext;
            _arguments = arguments;
            _returnValue = returnValue;
            _outputs = new TransparentProxyOutputParameterCollection(callMessage, arguments);
        }

        /// <summary>
        /// Creates a new <see cref="TransparentProxyMethodReturn"/> object that contains an
        /// exception thrown by the target.
        /// </summary>
        /// <param name="ex">Exception that was thrown.</param>
        /// <param name="callMessage">The original call message that invoked the method.</param>
        /// <param name="invocationContext">Invocation context dictionary passed into the call.</param>
        public TransparentProxyMethodReturn(Exception ex, IMethodCallMessage callMessage, IDictionary<string, object> invocationContext)
        {
            _callMessage = callMessage;
            _invocationContext = invocationContext;
            _exception = ex;
            _arguments = new object[0];
            _outputs = new ParameterCollection(_arguments, new ParameterInfo[0], pi => false);
        }

        /// <summary>
        /// The collection of output parameters. If the method has no output
        /// parameters, this is a zero-length list (never null).
        /// </summary>
        /// <value>The output parameter collection.</value>
        public IParameterCollection Outputs
        {
            [SecuritySafeCritical]
            get { return _outputs; }
        }

        /// <summary>
        /// Return value from the method call.
        /// </summary>
        /// <remarks>This value is null if the method has no return value.</remarks>
        /// <value>The return value.</value>
        public object ReturnValue
        {
            [SecuritySafeCritical]
            get { return _returnValue; }

            [SecuritySafeCritical]
            set
            {
                _returnValue = value;
            }
        }

        /// <summary>
        /// If the method threw an exception, the exception object is here.
        /// </summary>
        /// <value>The exception, or null if no exception was thrown.</value>
        public Exception Exception
        {
            [SecuritySafeCritical]
            get { return _exception; }
            
            [SecuritySafeCritical]
            set
            {
                _exception = value;
            }
        }

        /// <summary>
        /// Retrieves a dictionary that can be used to store arbitrary additional
        /// values. This allows the user to pass values between call handlers.
        /// </summary>
        /// <remarks>This is guaranteed to be the same dictionary that was used
        /// in the IMethodInvocation object, so handlers can set context
        /// properties in the pre-call phase and retrieve them in the after-call phase.
        /// </remarks>
        /// <value>The invocation context dictionary.</value>
        public IDictionary<string, object> InvocationContext
        {
            [SecuritySafeCritical]
            get { return _invocationContext; }
        }

        /// <summary>
        /// Constructs a <see cref="IMethodReturnMessage"/> for the remoting
        /// infrastructure based on the contents of this object.
        /// </summary>
        /// <returns>The <see cref="IMethodReturnMessage"/> instance.</returns>
        [SecurityCritical]
        public IMethodReturnMessage ToMethodReturnMessage()
        {
            if (_exception == null)
            {
                return
                    new ReturnMessage(_returnValue, _arguments, _arguments.Length,
                        _callMessage.LogicalCallContext, _callMessage);
            }
            return new ReturnMessage(_exception, _callMessage);
        }
    }
}
