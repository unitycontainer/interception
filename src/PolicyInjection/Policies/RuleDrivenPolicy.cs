

using System.Collections.Generic;
using Unity.Interception.Interceptors;
using Unity.Interception.PolicyInjection.MatchingRules;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.Utilities;

namespace Unity.Interception.PolicyInjection.Policies
{
    /// <summary>
    /// A policy is a combination of a matching rule set and a set of handlers.
    /// If the policy applies to a member, then the handlers will be enabled for
    /// that member.
    /// </summary>
    public class RuleDrivenPolicy : InjectionPolicy
    {
        private readonly MatchingRuleSet _ruleSet;
        private readonly IEnumerable<string> _callHandlerNames;

        /// <summary>
        /// Creates a new <see cref="RuleDrivenPolicy"/> object with a set of matching rules
        /// and the names to use when resolving handlers.
        /// </summary>
        public RuleDrivenPolicy(IMatchingRule[] matchingRules, string[] callHandlerNames)
            : this("Unnamed policy", matchingRules, callHandlerNames)
        {
        }

        /// <summary>
        /// Creates a new <see cref="RuleDrivenPolicy"/> object with a name, a set of matching rules
        /// and the names to use when resolving handlers.
        /// </summary>
        public RuleDrivenPolicy(string name, IMatchingRule[] matchingRules, string[] callHandlerNames)
            : base(name)
        {
            _ruleSet = new MatchingRuleSet();
            _ruleSet.AddRange(matchingRules);

            _callHandlerNames = callHandlerNames;
        }

        /// <summary>
        /// Checks if the rules in this policy match the given member info.
        /// </summary>
        /// <param name="member">MemberInfo to check against.</param>
        /// <returns>true if ruleset matches, false if it does not.</returns>
        protected override bool DoesMatch(MethodImplementationInfo member)
        {
            Guard.ArgumentNotNull(member, "member");

            bool matchesInterface = member.InterfaceMethodInfo != null ? _ruleSet.Matches(member.InterfaceMethodInfo) : false;
            bool matchesImplementation = _ruleSet.Matches(member.ImplementationMethodInfo);
            return matchesInterface | matchesImplementation;
        }

        /// <summary>
        /// Return ordered collection of handlers in order that apply to the given member.
        /// </summary>
        /// <param name="member">Member that may or may not be assigned handlers by this policy.</param>
        /// <param name="container">The <see cref="IUnityContainer"/> to use when creating handlers,
        /// if necessary.</param>
        /// <returns>Collection of handlers (possibly empty) that apply to this member.</returns>
        protected override IEnumerable<ICallHandler> DoGetHandlersFor(MethodImplementationInfo member, IUnityContainer container)
        {
            if (Matches(member))
            {
                foreach (string callHandlerName in _callHandlerNames)
                {
                    yield return container.Resolve<ICallHandler>(callHandlerName);
                }
            }
        }
    }
}
