using System;
using Unity.Builder;
using Unity.Interception.Interceptors.TypeInterceptors;
using Unity.Policy;

namespace Unity.Interception.ContainerIntegration.ObjectBuilder
{
    /// <summary>
    /// Interface that controls when and how types get intercepted.
    /// </summary>
    public interface ITypeInterceptionPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Interceptor to use to create type proxy
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        ITypeInterceptor GetInterceptor(IUnityContainer container);

        /// <summary>
        /// Cache for proxied type.
        /// </summary>
        Type ProxyType { get; set; }
    }

    public static class TypeInterceptionPolicyExtension
    {
        public static ITypeInterceptor GetInterceptor(this ITypeInterceptionPolicy policy, IBuilderContext context)
        {
            return policy.GetInterceptor(context.Container);
        }
    }
}
