using System.Reflection;
using System.Security;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Unity.Interception.Interceptors.InstanceInterceptors.TransparentProxyInterception
{
    /// <summary>
    /// A class that wraps the outputs of a <see cref="IMethodCallMessage"/> into the
    /// <see cref="IParameterCollection"/> interface.
    /// </summary>
    [SecurityCritical]
    internal class TransparentProxyOutputParameterCollection : ParameterCollection
    {
        /// <summary>
        /// Constructs a new <see cref="TransparentProxyOutputParameterCollection"/> that wraps the
        /// given method call and arguments.
        /// </summary>
        /// <param name="callMessage">The call message.</param>
        /// <param name="arguments">The arguments.</param>
        public TransparentProxyOutputParameterCollection(MethodInfo targetMethod, object[] arguments)
            : base(arguments, targetMethod.GetParameters(), parameterInfo => parameterInfo.ParameterType.IsByRef)
        {
        }
    }
}
