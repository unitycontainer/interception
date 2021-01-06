using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Permissions;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.Utilities;

namespace Unity.Interception.Interceptors.InstanceInterceptors.TransparentProxyInterception
{
    /// <summary>
    /// An implementation of <see cref="IMethodInvocation"/> that wraps the
    /// remoting based <see cref="IMethodCallMessage"/> in the PIAB call
    /// interface.
    /// </summary>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
    public sealed class TransparentProxyMethodInvocation : IMethodInvocation
    {
        private readonly IMethodCallMessage _callMessage;
        private readonly TransparentProxyInputParameterCollection _inputParams;
        private readonly ParameterCollection _allParams;
        private readonly Dictionary<string, object> _invocationContext;
        private readonly object _target;
        private readonly object[] _arguments;

        /// <summary>
        /// Creates a new <see cref="IMethodInvocation"/> implementation that wraps
        /// the given <paramref name="callMessage"/>, with the given ultimate
        /// target object.
        /// </summary>
        /// <param name="callMessage">Remoting call message object.</param>
        /// <param name="target">Ultimate target of the method call.</param>
        public TransparentProxyMethodInvocation(IMethodCallMessage callMessage, object target)
        {
            Guard.ArgumentNotNull(callMessage, "callMessage");

            _callMessage = callMessage;
            _invocationContext = new Dictionary<string, object>();
            _target = target;
            _arguments = callMessage.Args;
            _inputParams = new TransparentProxyInputParameterCollection(callMessage, _arguments);
            _allParams =
                new ParameterCollection(_arguments, callMessage.MethodBase.GetParameters(), info => true);
        }

        /// <summary>
        /// Gets the inputs for this call.
        /// </summary>
        /// <value>The input collection.</value>
        public IParameterCollection Inputs
        {
            [SecuritySafeCritical]
            get { return _inputParams; }
        }

        /// <summary>
        /// Collection of all parameters to the call: in, out and byref.
        /// </summary>
        /// <value>The arguments collection.</value>
        IParameterCollection IMethodInvocation.Arguments
        {
            [SecuritySafeCritical]
            get { return _allParams; }
        }

        /// <summary>
        /// Retrieves a dictionary that can be used to store arbitrary additional
        /// values. This allows the user to pass values between call handlers.
        /// </summary>
        /// <value>The invocation context dictionary.</value>
        public IDictionary<string, object> InvocationContext
        {
            [SecuritySafeCritical]
            get { return _invocationContext; }
        }

        /// <summary>
        /// The object that the call is made on.
        /// </summary>
        /// <value>The target object.</value>
        public object Target
        {
            [SecuritySafeCritical]
            get { return _target; }
        }

        /// <summary>
        /// The method on Target that we're aiming at.
        /// </summary>
        /// <value>The target method base.</value>
        public MethodBase MethodBase
        {
            [SecuritySafeCritical]
            get { return _callMessage.MethodBase; }
        }

        /// <summary>
        /// Factory method that creates the correct implementation of
        /// IMethodReturn.
        /// </summary>
        /// <remarks>In this implementation we create an instance of <see cref="TransparentProxyMethodReturn"/>.</remarks>
        /// <param name="returnValue">Return value to be placed in the IMethodReturn object.</param>
        /// <param name="outputs">All arguments passed or returned as out/byref to the method. 
        /// Note that this is the entire argument list, including in parameters.</param>
        /// <returns>New IMethodReturn object.</returns>
        [SecuritySafeCritical]
        public IMethodReturn CreateMethodReturn(object returnValue, params object[] outputs)
        {
            return new TransparentProxyMethodReturn(_callMessage, returnValue, outputs, _invocationContext);
        }

        /// <summary>
        /// Factory method that creates the correct implementation of
        /// IMethodReturn in the presence of an exception.
        /// </summary>
        /// <param name="ex">Exception to be set into the returned object.</param>
        /// <returns>New IMethodReturn object</returns>
        [SecuritySafeCritical]
        public IMethodReturn CreateExceptionMethodReturn(Exception ex)
        {
            return new TransparentProxyMethodReturn(ex, _callMessage, _invocationContext);
        }

        /// <summary>
        /// Gets the collection of arguments being passed to the target.
        /// </summary>
        /// <remarks>This method exists because the underlying remoting call message
        /// does not let handlers change the arguments.</remarks>
        /// <value>Array containing the arguments to the target.</value>
        internal object[] Arguments
        {
            get { return _arguments; }
        }
    }
}
