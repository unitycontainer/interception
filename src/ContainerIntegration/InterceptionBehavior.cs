﻿using System;
using Unity.Interception.ContainerIntegration.ObjectBuilder;
using Unity.Interception.InterceptionBehaviors;

namespace Unity.Interception.ContainerIntegration
{
    /// <summary>
    /// Stores information about a single <see cref="IInterceptionBehavior"/> to be used on an intercepted object and
    /// configures a container accordingly.
    /// </summary>
    /// <seealso cref="IInterceptionBehavior"/>
    public class InterceptionBehavior : InterceptionBehaviorBase
    {
        public override bool BuildRequired => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// <see cref="IInterceptionBehavior"/>.
        /// </summary>
        /// <param name="interceptionBehavior">The interception behavior to use.</param>
        public InterceptionBehavior(IInterceptionBehavior interceptionBehavior)
            : base(interceptionBehavior)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// given type/name pair.
        /// </summary>
        /// <param name="behaviorType">Type of behavior to </param>
        /// <param name="name"></param>
        public InterceptionBehavior(Type behaviorType, string name)
            : base(behaviorType, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// given behavior type.
        /// </summary>
        /// <param name="behaviorType">Type of behavior to </param>
        public InterceptionBehavior(Type behaviorType)
            : base(behaviorType)
        {
        }

        /// <summary>
        /// GetOrDefault the list of behaviors for the current type so that it can be added to.
        /// </summary>
        /// <param name="policies">Policy list.</param>
        /// <returns>An instance of <see cref="InterceptionBehaviorsPolicy"/>.</returns>
        protected override InterceptionBehaviorsPolicy GetBehaviorsPolicy<TPolicySet>(ref TPolicySet policies)
        {
            return InterceptionBehaviorsPolicy.GetOrCreate(ref policies);
        }

        protected override string ToString(bool debug = false)
        {
            throw new NotImplementedException();
        }

        public override void Validate(Type type)
        {
            throw new NotImplementedException();
        }
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
        public InterceptionBehavior() : base(typeof(TBehavior)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// given type/name pair.
        /// </summary>
        /// <param name="name">Name to use to resolve the behavior.</param>
        public InterceptionBehavior(string name) : base(typeof(TBehavior), name) { }
    }
}
