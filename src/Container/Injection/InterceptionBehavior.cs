using System;
using System.Linq;
using Unity.Extension;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.InterceptionBehaviors;
using Unity.Resolution;

namespace Unity.Interception
{
    /// <summary>
    /// Stores information about a single <see cref="IInterceptionBehavior"/> to be used on an intercepted object and
    /// configures a container accordingly.
    /// </summary>
    /// <seealso cref="IInterceptionBehavior"/>
    public class InterceptionBehavior : InterceptionMember
    {
        #region Fields

        private readonly Type _type;
        private readonly string? _name;
        private IInterceptionBehavior? _explicitBehavior;

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IInterceptionBehavior"/> with a 
        /// <see cref="IInterceptionBehavior"/>.
        /// </summary>
        /// <param name="interceptionBehavior">The interception behavior to use.</param>
        public InterceptionBehavior(IInterceptionBehavior interceptionBehavior)
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
        public InterceptionBehavior(Type behaviorType, string? name)
        {
            _type = behaviorType ?? throw new ArgumentNullException(nameof(behaviorType));
            _name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// given behavior type.
        /// </summary>
        /// <param name="behaviorType">Type of behavior to </param>
        public InterceptionBehavior(Type behaviorType)
            : this(behaviorType, null)
        {
        }

        #endregion


        #region Injection

        public override MatchRank Matches(Type other)
        {
            if (_type.Equals(other)) return MatchRank.ExactMatch;

            if (other.IsAssignableFrom(_type)) return MatchRank.Compatible;

            return MatchRank.NoMatch;
        }
        
        #endregion


        #region Behavior

        public virtual IInterceptionBehavior GetBehavior(Interception extension)
            => _explicitBehavior ??= (IInterceptionBehavior)extension.Container!.Resolve(_type, _name)!;


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

        #endregion
    }

    /// <summary>
    /// A generic version of <see cref="InterceptionBehavior"/> that lets you
    /// specify behavior types using generic syntax.
    /// </summary>
    /// <typeparam name="TBehavior">Type of behavior to register.</typeparam>
    public class InterceptionBehavior<TBehavior> : InterceptionBehavior
        where TBehavior : IInterceptionBehavior
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// given behavior type.
        /// </summary>
        public InterceptionBehavior() 
            : base(typeof(TBehavior)) 
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// given type/name pair.
        /// </summary>
        /// <param name="name">Name to use to resolve the behavior.</param>
        public InterceptionBehavior(string name) 
            : base(typeof(TBehavior), name) 
        { }
    }


    public static class InterceptionBehaviorExtensions
    {
        //public static IEnumerable<IInterceptionBehavior> GetBehavior<TContext>(ref TContext context)
        //{ 
            
        //}

    }
}
