using System;
using Unity.Builder;
using Unity.Interception.ContainerIntegration.ObjectBuilder;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Utilities;
using Unity.Policy;

namespace Unity.Interception.ContainerIntegration
{
    /// <summary>
    /// Base class for injection members that allow you to add
    /// interception behaviors.
    /// </summary>
    public abstract class InterceptionBehaviorBase : InterceptionMember
    {
        private readonly NamedTypeBuildKey _behaviorKey;
        private readonly IInterceptionBehavior _explicitBehavior;

        /// <summary>
        /// Initializes a new instance of the <see cref="IInterceptionBehavior"/> with a 
        /// <see cref="IInterceptionBehavior"/>.
        /// </summary>
        /// <param name="interceptionBehavior">The interception behavior to use.</param>
        protected InterceptionBehaviorBase(IInterceptionBehavior interceptionBehavior)
        {
            _explicitBehavior = interceptionBehavior ?? throw new ArgumentNullException(nameof(interceptionBehavior));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// given type/name pair.
        /// </summary>
        /// <param name="behaviorType">Type of behavior to </param>
        /// <param name="name"></param>
        protected InterceptionBehaviorBase(Type behaviorType, string name)
        {
            Guard.ArgumentNotNull(behaviorType, "behaviorType");
            Guard.TypeIsAssignable(typeof(IInterceptionBehavior), behaviorType, "behaviorType");
            _behaviorKey = new NamedTypeBuildKey(behaviorType, name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// given behavior type.
        /// </summary>
        /// <param name="behaviorType">Type of behavior to </param>
        protected InterceptionBehaviorBase(Type behaviorType)
            : this(behaviorType, null)
        {
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the container to use the represented 
        /// <see cref="IInterceptionBehavior"/> for the supplied parameters.
        /// </summary>
        /// <param name="registeredType">Interface being registered.</param>
        /// <param name="mappedToType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies<TContext, TPolicySet>(Type registeredType, Type mappedToType, string name, ref TPolicySet policies)
        {
            if (_explicitBehavior != null)
            {
                var behaviorsPolicy = GetBehaviorsPolicy(ref policies);
                behaviorsPolicy.AddBehavior(_explicitBehavior);
            }
            else
            {
                var behaviorsPolicy = GetBehaviorsPolicy(ref policies);
                behaviorsPolicy.AddBehaviorKey(_behaviorKey);
            }
        }

        /// <summary>
        /// GetOrDefault the list of behaviors for the current type so that it can be added to.
        /// </summary>
        /// <param name="policies">Policy list.</param>
        /// <returns>An instance of <see cref="InterceptionBehaviorsPolicy"/>.</returns>
        protected abstract InterceptionBehaviorsPolicy GetBehaviorsPolicy<TPolicySet>(ref TPolicySet policies)
            where TPolicySet : IPolicySet;
    }
}
