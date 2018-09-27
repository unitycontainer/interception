

using System;

namespace Unity.Interception.Interceptors.TypeInterceptors
{
    /// <summary>
    /// Interface for interceptor objects that generate
    /// proxy types.
    /// </summary>
    public interface ITypeInterceptor : IInterceptor
    {
        /// <summary>
        /// Create a type to proxy for the given type <paramref name="t"/>.
        /// </summary>
        /// <param name="t">Type to proxy.</param>
        /// <param name="additionalInterfaces">Additional interfaces the proxy must implement.</param>
        /// <returns>New type that can be instantiated instead of the
        /// original type t, and supports interception.</returns>
        Type CreateProxyType(Type t, params Type[] additionalInterfaces);
    }
}
