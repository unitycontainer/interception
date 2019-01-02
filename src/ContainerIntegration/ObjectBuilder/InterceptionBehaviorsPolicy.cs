using System;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Interception.ContainerIntegration.ObjectBuilder
{
    /// <summary>
    /// An <see cref="IInterceptionBehaviorsPolicy"/> that accumulates a sequence of 
    /// <see cref="IInterceptionBehavior"/> instances for an intercepted object.
    /// </summary>
    public class InterceptionBehaviorsPolicy : IInterceptionBehaviorsPolicy
    {
        private readonly List<NamedTypeBuildKey> _behaviorKeys = new List<NamedTypeBuildKey>();

        /// <summary>
        /// GetOrDefault the set of <see cref="NamedTypeBuildKey"/> that can be used to resolve the
        /// behaviors.
        /// </summary>
        public IEnumerable<NamedTypeBuildKey> BehaviorKeys => _behaviorKeys;

        private readonly List<IInterceptionBehavior> _explicitBehaviors = new List<IInterceptionBehavior>();

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
        public IEnumerable<IInterceptionBehavior> GetEffectiveBehaviors(
            IUnityContainer container, IInterceptor interceptor, Type typeToIntercept, Type implementationType)
        {
            var interceptionRequest = new CurrentInterceptionRequest(interceptor, typeToIntercept, implementationType);

            foreach (var behavior in _explicitBehaviors)
            {
                yield return behavior;
            }

            foreach (var key in BehaviorKeys)
            {
                var behavior = (IInterceptionBehavior)container.Resolve(key.Type, key.Name, new DependencyOverride<CurrentInterceptionRequest>(interceptionRequest));
                yield return behavior;
            }
        }

        internal void AddBehaviorKey(NamedTypeBuildKey key)
        {
            _behaviorKeys.Add(key);
        }

        public void AddBehavior(IInterceptionBehavior behavior)
        {
            _explicitBehaviors.Add(behavior);
        }

        internal static InterceptionBehaviorsPolicy GetOrCreate<TPolicySet>(ref TPolicySet policies)
            where TPolicySet : IPolicySet
        {
            IInterceptionBehaviorsPolicy policy = (IInterceptionBehaviorsPolicy)policies.Get(typeof(IInterceptionBehaviorsPolicy));

            if (!(policy is InterceptionBehaviorsPolicy))
            {
                policy = new InterceptionBehaviorsPolicy();
                policies.Set(typeof(IInterceptionBehaviorsPolicy), policy);
            }
            return (InterceptionBehaviorsPolicy)policy;
        }
    }
}
