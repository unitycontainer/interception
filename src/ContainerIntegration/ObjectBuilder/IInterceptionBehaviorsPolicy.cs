using System;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors;
using Unity.Policy;

namespace Unity.Interception.ContainerIntegration.ObjectBuilder
{
    /// <summary>
    /// An <see cref="IBuilderPolicy"/> that returns a sequence of <see cref="IInterceptionBehavior"/> 
    /// instances for an intercepted object.
    /// </summary>
    public interface IInterceptionBehaviorsPolicy : IBuilderPolicy
    {
        /// <summary>
        /// GetOrDefault the set of <see cref="NamedTypeBuildKey"/> that can be used to resolve the
        /// behaviors.
        /// </summary>
        IEnumerable<NamedTypeBuildKey> BehaviorKeys { get; }

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
        public static IEnumerable<IInterceptionBehavior> GetEffectiveBehaviors(this IInterceptionBehaviorsPolicy policy, 
                                                                      IBuilderContext context, IInterceptor interceptor,
                                                                          Type typeToIntercept, Type implementationType)
        {
            return policy.GetEffectiveBehaviors(context.Container, interceptor, typeToIntercept, implementationType);
        }
    }
}
