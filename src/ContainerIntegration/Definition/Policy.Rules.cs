using System;
using Unity.Injection;
using Unity.Interception.PolicyInjection.MatchingRules;
using Unity.Lifetime;

namespace Unity.Interception
{
    public partial class PolicyDefinition
    {
        /// <summary>
        /// Adds a reference to matching rule by name.
        /// </summary>
        /// <param name="name">The name for the matching rule.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        /// <remarks>
        /// The details of how the rule should be created by the container must be specified using a 
        /// standard injection specification mechanism.
        /// </remarks>
        public PolicyDefinition AddMatchingRule(string name)
        {
            return AddElement<IMatchingRule>(name, UpdateRulesParameters);
        }

        /// <summary>
        /// Makes <paramref name="instance"/> a matching rule in the current policy.
        /// </summary>
        /// <param name="instance">The new <see cref="IMatchingRule"/> for the policy.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddMatchingRule(IMatchingRule instance)
        {
            return AddElement(instance, UpdateRulesParameters);
        }

        /// <summary>
        /// Configures injection for a new <see cref="IMatchingRule"/> and makes it available
        /// as a matching rule in the current policy.
        /// </summary>
        /// <param name="type">The type for the new matching rule.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddMatchingRule(
            Type type,
            params InjectionMember[] injectionMembers)
        {
            return AddElement<IMatchingRule>(
                type,
                NewName(),
                null,
                injectionMembers,
                UpdateRulesParameters);
        }

        /// <summary>
        /// Configures injection for a new <see cref="IMatchingRule"/> and makes it available
        /// as a matching rule in the current policy, using the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <param name="type">The type for the new matching rule.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the configured matching rule.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddMatchingRule(
            Type type,
            LifetimeManager lifetimeManager,
            params InjectionMember[] injectionMembers)
        {
            return AddElement<IMatchingRule>(
                type,
                NewName(),
                lifetimeManager,
                injectionMembers,
                UpdateRulesParameters);
        }

        /// <summary>
        /// Configures injection for a new <see cref="IMatchingRule"/> using the specified name
        /// and makes it available as a matching rule in the current policy.
        /// </summary>
        /// <param name="type">The type for the new matching rule.</param>
        /// <param name="name">The name for the injection configuration for the matching rule.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddMatchingRule(
            Type type,
            string name,
            params InjectionMember[] injectionMembers)
        {
            return AddElement<IMatchingRule>(
                type,
                name,
                null,
                injectionMembers,
                UpdateRulesParameters);
        }

        /// <summary>
        /// Configures injection for a new <see cref="IMatchingRule"/> and makes it available
        /// as a matching rule in the current policy, using the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <param name="type">The type for the new matching rule.</param>
        /// <param name="name">The name for the injection configuration for the matching rule.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the configured matching rule.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddMatchingRule(
            Type type,
            string name,
            LifetimeManager lifetimeManager,
            params InjectionMember[] injectionMembers)
        {
            return AddElement<IMatchingRule>(
                type,
                name,
                lifetimeManager,
                injectionMembers,
                UpdateRulesParameters);
        }

        /// <summary>
        /// Configures injection for a new <see cref="IMatchingRule"/> and makes it available
        /// as a matching rule in the current policy.
        /// </summary>
        /// <typeparam name="TMatchingRule">The type for the new matching rule.</typeparam>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddMatchingRule<TMatchingRule>(
            params InjectionMember[] injectionMembers)
            where TMatchingRule : IMatchingRule
        {
            return AddElement<IMatchingRule, TMatchingRule>(
                NewName(),
                null,
                injectionMembers,
                UpdateRulesParameters);
        }

        /// <summary>
        /// Configures injection for a new <see cref="IMatchingRule"/> and makes it available
        /// as a matching rule in the current policy, using the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <typeparam name="TMatchingRule">The type for the new matching rule.</typeparam>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the configured matching rule.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddMatchingRule<TMatchingRule>(
            LifetimeManager lifetimeManager,
            params InjectionMember[] injectionMembers)
            where TMatchingRule : IMatchingRule
        {
            return AddElement<IMatchingRule, TMatchingRule>(
                NewName(),
                lifetimeManager,
                injectionMembers,
                UpdateRulesParameters);
        }

        /// <summary>
        /// Configures injection for a new <see cref="IMatchingRule"/> using the specified name
        /// and makes it available as a matching rule in the current policy.
        /// </summary>
        /// <typeparam name="TMatchingRule">The type for the new matching rule.</typeparam>
        /// <param name="name">The name for the injection configuration for the matching rule.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddMatchingRule<TMatchingRule>(
            string name,
            params InjectionMember[] injectionMembers)
            where TMatchingRule : IMatchingRule
        {
            return AddElement<IMatchingRule, TMatchingRule>(
                name,
                null,
                injectionMembers,
                UpdateRulesParameters);
        }

        /// <summary>
        /// Configures injection for a new <see cref="IMatchingRule"/> using the specified name
        /// and makes it available as a matching rule in the current policy, 
        /// using the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <typeparam name="TMatchingRule">The type for the new matching rule.</typeparam>
        /// <param name="name">The name for the injection configuration for the matching rule.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the configured matching rule.</param>
        /// <param name="injectionMembers">Objects containing the details on which members to inject and how.</param>
        /// <returns>
        /// The <see cref="PolicyDefinition"/> than allows further configuration of the policy.
        /// </returns>
        public PolicyDefinition AddMatchingRule<TMatchingRule>(
            string name,
            LifetimeManager lifetimeManager,
            params InjectionMember[] injectionMembers)
            where TMatchingRule : IMatchingRule
        {
            return AddElement<IMatchingRule, TMatchingRule>(
                name,
                lifetimeManager,
                injectionMembers,
                UpdateRulesParameters);
        }
    }
}
