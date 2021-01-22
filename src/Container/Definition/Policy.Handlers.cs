using System;
using Unity.Injection;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Lifetime;

namespace Unity.Interception
{
    /// <summary>
    /// Transient class that supports convenience method for specifying interception policies.
    /// </summary>
    public partial class PolicyDefinition
    {
        /// <summary>
        /// Adds a reference to call handler by name.
        /// </summary>
        /// <param name="name">The name for the call handler.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        /// <remarks>
        /// The details of how the handler should be created by the container must be specified using a 
        /// standard injection specification mechanism.
        /// </remarks>
        public PolicyDefinition AddCallHandler(string name)
        {
            _handlers.Add(new ResolvedParameter<ICallHandler>(name));
            return this;
        }

        /// <summary>
        /// Makes <paramref name="instance"/> a call handler in the current policy.
        /// </summary>
        /// <param name="instance">The new <see cref="ICallHandler"/> for the policy.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddCallHandler(ICallHandler instance, bool register = false)
        {
            _handlers.Add(instance!);

            if (!register) return this;

            return AddElement(instance);
        }

        /// <summary>
        /// Configures injection for a new <see cref="ICallHandler"/> and makes it available
        /// as a call handler in the current policy.
        /// </summary>
        /// <param name="type">The type for the new call handler.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddCallHandler(Type type, params InjectionMember[] injectionMembers)
            => AddElement<ICallHandler>(type, Guid.NewGuid().ToString(), null, injectionMembers);

        /// <summary>
        /// Configures injection for a new <see cref="ICallHandler"/> and makes it available
        /// as a call handler in the current policy, using the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <param name="type">The type for the new call handler.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the configured call handler.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddCallHandler(Type type, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
            => AddElement<ICallHandler>(type, Guid.NewGuid().ToString(), lifetimeManager, injectionMembers);

        /// <summary>
        /// Configures injection for a new <see cref="ICallHandler"/> using the specified name
        /// and makes it available as a call handler in the current policy.
        /// </summary>
        /// <param name="type">The type for the new call handler.</param>
        /// <param name="name">The name for the injection configuration for the call handler.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddCallHandler(Type type, string name, params InjectionMember[] injectionMembers)
            => AddElement<ICallHandler>(type, name, null, injectionMembers);

        /// <summary>
        /// Configures injection for a new <see cref="ICallHandler"/> using the specified name
        /// and makes it available as a call handler in the current policy, 
        /// using the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <param name="type">The type for the new call handler.</param>
        /// <param name="name">The name for the injection configuration for the call handler.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the configured call handler.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddCallHandler(Type type, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
            => AddElement<ICallHandler>(type, name, lifetimeManager, injectionMembers);

        /// <summary>
        /// Configures injection for a new <see cref="ICallHandler"/> and makes it available
        /// as a call handler in the current policy.
        /// </summary>
        /// <typeparam name="TCallHandler">The type for the new call handler.</typeparam>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddCallHandler<TCallHandler>(params InjectionMember[] injectionMembers)
            where TCallHandler : ICallHandler
            => AddElement<ICallHandler, TCallHandler>(Guid.NewGuid().ToString(), null, injectionMembers);

        /// <summary>
        /// Configures injection for a new <see cref="ICallHandler"/> and makes it available
        /// as a call handler in the current policy, using the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <typeparam name="TCallHandler">The type for the new call handler.</typeparam>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the configured call handler.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddCallHandler<TCallHandler>(LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
            where TCallHandler : ICallHandler 
            => AddElement<ICallHandler, TCallHandler>(Guid.NewGuid().ToString(), lifetimeManager, injectionMembers);

        /// <summary>
        /// Configures injection for a new <see cref="ICallHandler"/> using the specified name
        /// and makes it available as a call handler in the current policy.
        /// </summary>
        /// <typeparam name="TCallHandler">The type for the new call handler.</typeparam>
        /// <param name="name">The name for the injection configuration for the call handler .</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddCallHandler<TCallHandler>(string name, params InjectionMember[] injectionMembers)
            where TCallHandler : ICallHandler 
            => AddElement<ICallHandler, TCallHandler>(name, null, injectionMembers);

        /// <summary>
        /// Configures injection for a new <see cref="ICallHandler"/> using the specified name
        /// and makes it available as a call handler in the current policy, 
        /// using the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <typeparam name="TCallHandler">The type for the new call handler.</typeparam>
        /// <param name="name">The name for the injection configuration for the call handler .</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the configured call handler.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddCallHandler<TCallHandler>(string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
            where TCallHandler : ICallHandler 
            => AddElement<ICallHandler, TCallHandler>(name, lifetimeManager, injectionMembers);
    }
}
