using System;
using System.Collections.Generic;

namespace Unity.Interception.Interceptors.InstanceInterceptors.TransparentProxyInterception
{
    /// <summary>
    /// An instance interceptor that uses remoting proxies to do the
    /// interception.
    /// </summary>
    [Obsolete("Due to discontinued support for Remoting in .NET 5.0 and up, Transparent interception is deprecated", true)]
    public class TransparentProxyInterceptor : IInstanceInterceptor
    {
        public bool CanIntercept(Type t) 
            => throw new NotImplementedException();

        public IEnumerable<MethodImplementationInfo> GetInterceptableMethods(Type interceptedType, Type implementationType)
            => throw new NotImplementedException();


        /// <summary>
        /// Create a proxy object that provides interception for <paramref name="target"/>.
        /// </summary>
        /// <param name="type">Type to generate the proxy of.</param>
        /// <param name="target">Object to create the proxy for.</param>
        /// <param name="interfaces">Additional interfaces the proxy must implement.</param>
        /// <returns>The proxy object.</returns>
        public IInterceptingProxy CreateProxy(Type type, object target, params Type[] interfaces)
            => throw new NotImplementedException();
    }
}
