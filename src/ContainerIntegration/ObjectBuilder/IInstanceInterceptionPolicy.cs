

using Unity.Builder;
using Unity.Interception.Interceptors.InstanceInterceptors;
using Unity.Policy;

namespace Unity.Interception.ContainerIntegration.ObjectBuilder
{
    /// <summary>
    /// An interface that determines when to intercept instances
    /// and which interceptor to use.
    /// </summary>
    public interface IInstanceInterceptionPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Interceptor to use.
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        IInstanceInterceptor GetInterceptor(IBuilderContext context);
    }
}
