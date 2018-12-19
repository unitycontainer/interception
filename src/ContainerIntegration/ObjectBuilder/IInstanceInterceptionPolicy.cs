using Unity.Builder;
using Unity.Interception.Interceptors.InstanceInterceptors;

namespace Unity.Interception.ContainerIntegration.ObjectBuilder
{
    /// <summary>
    /// An interface that determines when to intercept instances
    /// and which interceptor to use.
    /// </summary>
    public interface IInstanceInterceptionPolicy 
    {
        /// <summary>
        /// Interceptor to use.
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        IInstanceInterceptor GetInterceptor(ref BuilderContext context);
    }
}
