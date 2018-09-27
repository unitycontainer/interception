

using Unity.Builder;
using Unity.Interception.Interceptors.InstanceInterceptors;

namespace Unity.Interception.ContainerIntegration.ObjectBuilder
{
    /// <summary>
    /// Implementation of <see cref="IInstanceInterceptionPolicy"/> that returns a
    /// pre-created interceptor.
    /// </summary>
    public class FixedInstanceInterceptionPolicy : IInstanceInterceptionPolicy
    {
        private readonly IInstanceInterceptor _interceptor;

        /// <summary>
        /// Create a new instance of <see cref="FixedInstanceInterceptionPolicy"/>.
        /// </summary>
        /// <param name="interceptor">Interceptor to store.</param>
        public FixedInstanceInterceptionPolicy(IInstanceInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        /// <summary>
        /// Interceptor to use.
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        public IInstanceInterceptor GetInterceptor(IBuilderContext context)
        {
            return _interceptor;
        }
    }
}
