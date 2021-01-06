using System;
using Unity.Extension;
using Unity.Interception.ContainerIntegration.ObjectBuilder;
using Unity.Interception.InterceptionBehaviors;

namespace Unity.Interception.ContainerIntegration
{
    /// <summary>
    /// Base class for injection members that allow you to add
    /// interception behaviors.
    /// </summary>
    public abstract class InterceptionBehaviorBase : InterceptionMember // TODO: IAddPolicies
    {
        private readonly Contract _behaviorKey;
        private readonly IInterceptionBehavior _explicitBehavior;

        /// <summary>
        /// Initializes a new instance of the <see cref="IInterceptionBehavior"/> with a 
        /// <see cref="IInterceptionBehavior"/>.
        /// </summary>
        /// <param name="interceptionBehavior">The interception behavior to use.</param>
        protected InterceptionBehaviorBase(IInterceptionBehavior interceptionBehavior)
        {
            _explicitBehavior = interceptionBehavior ?? throw new ArgumentNullException(nameof(interceptionBehavior));
            _behaviorKey = new Contract(interceptionBehavior.GetType(), null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// given type/name pair.
        /// </summary>
        /// <param name="behaviorType">Type of behavior to </param>
        /// <param name="name"></param>
        protected InterceptionBehaviorBase(Type behaviorType, string name)
        {
            _behaviorKey = new Contract(
                behaviorType ?? throw new ArgumentNullException(nameof(behaviorType)),
                name         ?? throw new ArgumentNullException(nameof(name)));
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

        public void AddPolicies<TPolicySet>(Type type, string name, ref TPolicySet policies)
            where TPolicySet : IPolicySet
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

        public override MatchRank Match(Type other)
        {
            if (_behaviorKey.Type.Equals(other)) return MatchRank.ExactMatch;

            if (other.IsAssignableFrom(_behaviorKey.Type)) return MatchRank.Compatible;

            return MatchRank.NoMatch;
        }

        /// <summary>
        /// GetOrDefault the list of behaviors for the current type so that it can be added to.
        /// </summary>
        /// <param name="policies">Policy list.</param>
        /// <returns>An instance of <see cref="InterceptionBehaviorsPolicy"/>.</returns>
        protected abstract InterceptionBehaviorsPolicy GetBehaviorsPolicy<TPolicySet>(ref TPolicySet policies)
            where TPolicySet : IPolicySet;

        public override bool BuildRequired => false;
    }
}
