

using System;

namespace Unity.Interception.Interceptors.InstanceInterceptors
{
    /// <summary>
    /// Interface for interceptors that generate separate proxy
    /// objects to implement interception on instances.
    /// </summary>
    public interface IInstanceInterceptor : IInterceptor
    {
        /// <summary>
        /// Create a proxy object that provides interception for <paramref name="target"/>.
        /// </summary>
        /// <param name="t">Type to generate the proxy of.</param>
        /// <param name="target">Object to create the proxy for.</param>
        /// <param name="additionalInterfaces">Additional interfaces the proxy must implement.</param>
        /// <returns>The proxy object.</returns>
        IInterceptingProxy CreateProxy(Type t, object target, params Type[] additionalInterfaces);
    }
}
