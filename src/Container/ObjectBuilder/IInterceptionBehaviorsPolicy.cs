using System;
using System.Collections.Generic;
using Unity.Extension;
using Unity.Interception.Interceptors;

namespace Unity.Interception.ContainerIntegration.ObjectBuilder
{
    /// <summary>
    /// An policy that returns a sequence of <see cref="IInterceptionBehavior"/> 
    /// instances for an intercepted object.
    /// </summary>
    public interface IInterceptionBehaviorsPolicy 
    {
        /// <summary>
        /// GetOrDefault the set of <see cref="NamedTypeBuildKey"/> that can be used to resolve the
        /// behaviors.
        /// </summary>
        IEnumerable<Contract> BehaviorKeys { get; }

        /// <summary>
        /// GetOrDefault the set of <see cref="IInterceptionBehavior"/> object to be used for the given type and
        /// interceptor.
        /// </summary>
        /// <remarks>
        /// This method will return a sequence of <see cref="IInterceptionBehavior"/>s. These behaviors will
        /// only be included if their <see cref="IInterceptionBehavior.WillExecute"/> properties are true.
        /// </remarks>
        /// <param name="container">Container context for the current build operation.</param>
        /// <param name="interceptor">Interceptor that will be used to invoke the behavior.</param>
        /// <param name="typeToIntercept">Type that interception was requested on.</param>
        /// <param name="implementationType">Type that implements the interception.</param>
        /// <returns></returns>
        IEnumerable<IInterceptionBehavior> GetEffectiveBehaviors(IUnityContainer container, IInterceptor interceptor,
                                                                 Type typeToIntercept, Type implementationType);
    }


    public static class InterceptionBehaviorsPolicyExtension
    {
        /// <summary>
        /// GetOrDefault the set of <see cref="IInterceptionBehavior"/> object to be used for the given type and
        /// interceptor.
        /// </summary>
        /// <remarks>
        /// This method will return a sequence of <see cref="IInterceptionBehavior"/>s. These behaviors will
        /// only be included if their <see cref="IInterceptionBehavior.WillExecute"/> properties are true.
        /// </remarks>
        /// <param name="context">Context for the current build operation.</param>
        /// <param name="interceptor">Interceptor that will be used to invoke the behavior.</param>
        /// <param name="typeToIntercept">Type that interception was requested on.</param>
        /// <param name="implementationType">Type that implements the interception.</param>
        /// <returns></returns>
        public static IEnumerable<IInterceptionBehavior> GetEffectiveBehaviors<TContext>(this IInterceptionBehaviorsPolicy policy, 
                                                                      ref TContext context, IInterceptor interceptor,
                                                                          Type typeToIntercept, Type implementationType)
            where TContext : IBuilderContext
        {
            return policy.GetEffectiveBehaviors(context.Container, interceptor, typeToIntercept, implementationType);
        }
    }
}
