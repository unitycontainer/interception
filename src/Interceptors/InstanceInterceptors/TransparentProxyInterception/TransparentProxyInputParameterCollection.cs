using System.Reflection;
using System.Security;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Unity.Interception.Interceptors.InstanceInterceptors.TransparentProxyInterception
{
    /// <summary>
    /// A class that wraps the inputs of a <see cref="IMethodCallMessage"/> into the
    /// <see cref="IParameterCollection"/> interface.
    /// </summary>
    [SecurityCritical]
    internal class TransparentProxyInputParameterCollection : ParameterCollection
    {
        /// <summary>
        /// Constructs a new <see cref="TransparentProxyInputParameterCollection"/> that wraps the
        /// given method call and arguments.
        /// </summary>
        /// <param name="callMessage">The call message.</param>
        /// <param name="arguments">The arguments.</param>
        public TransparentProxyInputParameterCollection(MethodInfo targetMethod, object[] arguments)
            : base(arguments, targetMethod.GetParameters(),
                delegate(ParameterInfo info) { return !info.IsOut; })
        {
        }
    }
}
