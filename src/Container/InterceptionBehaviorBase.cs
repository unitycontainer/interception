using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Extension;
using Unity.Interception.ContainerIntegration.ObjectBuilder;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors;
using Unity.Resolution;

namespace Unity.Interception.ContainerIntegration
{
    /// <summary>
    /// Base class for injection members that allow you to add
    /// interception behaviors.
    /// </summary>
    public abstract class InterceptionBehaviorBase : InterceptionMember
    {
        private readonly Type    _type;
        private readonly string? _name;

        private IInterceptionBehavior? _explicitBehavior;

        /// <summary>
        /// Initializes a new instance of the <see cref="IInterceptionBehavior"/> with a 
        /// <see cref="IInterceptionBehavior"/>.
        /// </summary>
        /// <param name="interceptionBehavior">The interception behavior to use.</param>
        protected InterceptionBehaviorBase(IInterceptionBehavior interceptionBehavior)
        {
            _explicitBehavior = interceptionBehavior ?? throw new ArgumentNullException(nameof(interceptionBehavior));
            _type = interceptionBehavior.GetType();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// given type/name pair.
        /// </summary>
        /// <param name="behaviorType">Type of behavior to </param>
        /// <param name="name"></param>
        protected InterceptionBehaviorBase(Type behaviorType, string? name)
        {
            _type = behaviorType ?? throw new ArgumentNullException(nameof(behaviorType));
            _name = name;
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


        public virtual IInterceptionBehavior GetBehavior<TContext>(ref TContext context)
            where TContext : IBuilderContext
            => _explicitBehavior ??= (IInterceptionBehavior)context.Resolve(_type, _name)!;


        public override MatchRank Matches(Type other)
        {
            if (_type.Equals(other)) return MatchRank.ExactMatch;

            if (other.IsAssignableFrom(_type)) return MatchRank.Compatible;

            return MatchRank.NoMatch;
        }

        public static IInterceptionBehavior[] GetEffectiveBehaviors<TContext>(ref TContext context, IInterceptor interceptor)
            where TContext : IBuilderContext
        {
            var instances = context.OfType<InterceptionBehavior>().ToArray();
            var behaviors = new IInterceptionBehavior[instances.Length];
            
            var interceptionRequest = new CurrentInterceptionRequest(interceptor, context.Contract.Type, context.Type);
            var @override = new DependencyOverride<CurrentInterceptionRequest>(interceptionRequest);

            for (var i = 0; i < instances.Length; i++)
            {
                var instance = instances[i];
                behaviors[i] = instance._explicitBehavior is not null
                    ? instance._explicitBehavior
                    : (IInterceptionBehavior)context.Container.Resolve(instance._type, instance._name, @override)!;
            }

            return behaviors;
        }

    }
}
