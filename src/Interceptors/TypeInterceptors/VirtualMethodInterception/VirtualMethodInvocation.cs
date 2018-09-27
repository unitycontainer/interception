

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.Utilities;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception
{
    /// <summary>
    /// Implementation of <see cref="IMethodInvocation"/> used
    /// by the virtual method interceptor.
    /// </summary>
    public class VirtualMethodInvocation : IMethodInvocation
    {
        private readonly ParameterCollection _inputs;
        private readonly ParameterCollection _arguments;
        private readonly Dictionary<string, object> _context;

        /// <summary>
        /// Construct a new <see cref="VirtualMethodInvocation"/> instance for the
        /// given target object and method, passing the <paramref name="parameterValues"/>
        /// to the target method.
        /// </summary>
        /// <param name="target">Object that is target of this invocation.</param>
        /// <param name="targetMethod">Method on <paramref name="target"/> to call.</param>
        /// <param name="parameterValues">Values for the parameters.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public VirtualMethodInvocation(object target, MethodBase targetMethod, params object[] parameterValues)
        {
            Guard.ArgumentNotNull(targetMethod, "targetMethod");

            Target = target;
            MethodBase = targetMethod;
            _context = new Dictionary<string, object>();

            ParameterInfo[] targetParameters = targetMethod.GetParameters();
            _arguments = new ParameterCollection(parameterValues, targetParameters, param => true);
            _inputs = new ParameterCollection(parameterValues, targetParameters, param => !param.IsOut);
        }

        /// <summary>
        /// Gets the inputs for this call.
        /// </summary>
        public IParameterCollection Inputs => _inputs;

        /// <summary>
        /// Collection of all parameters to the call: in, out and byref.
        /// </summary>
        public IParameterCollection Arguments => _arguments;

        /// <summary>
        /// Retrieves a dictionary that can be used to store arbitrary additional
        /// values. This allows the user to pass values between call handlers.
        /// </summary>
        public IDictionary<string, object> InvocationContext => _context;

        /// <summary>
        /// The object that the call is made on.
        /// </summary>
        public object Target { get; }

        /// <summary>
        /// The method on Target that we're aiming at.
        /// </summary>
        public MethodBase MethodBase { get; }

        /// <summary>
        /// Factory method that creates the correct implementation of
        /// IMethodReturn.
        /// </summary>
        /// <param name="returnValue">Return value to be placed in the IMethodReturn object.</param>
        /// <param name="outputs">All arguments passed or returned as out/byref to the method. 
        /// Note that this is the entire argument list, including in parameters.</param>
        /// <returns>New IMethodReturn object.</returns>
        public IMethodReturn CreateMethodReturn(object returnValue, params object[] outputs)
        {
            return new VirtualMethodReturn(this, returnValue, outputs);
        }

        /// <summary>
        /// Factory method that creates the correct implementation of
        /// IMethodReturn in the presence of an exception.
        /// </summary>
        /// <param name="ex">Exception to be set into the returned object.</param>
        /// <returns>New IMethodReturn object</returns>
        public IMethodReturn CreateExceptionMethodReturn(Exception ex)
        {
            return new VirtualMethodReturn(this, ex);
        }
    }
}
