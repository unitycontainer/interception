using System.Collections.Generic;
using System.Reflection;
using Unity.Interception.Interceptors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Unity.Interception
{
    /// <summary>
    /// A policy is a combination of a matching rule set and a set of handlers.
    /// If the policy applies to a member, then the handlers will be enabled for
    /// that member.
    /// </summary>
    public class RuleDrivenPolicy : InjectionPolicy
    {
        #region Fields

        private readonly IMatchingRule[] _rules;
        private readonly ICallHandler[] _handlers;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new <see cref="RuleDrivenPolicy"/> object with a name, a set of matching rules
        /// and the names to use when resolving handlers.
        /// </summary>
        public RuleDrivenPolicy(string name, IMatchingRule[] rules, ICallHandler[] handlers)
            : base(name)
        {
            _rules = rules;

            _handlers = handlers;
        }


        #endregion

        /// <summary>
        /// Checks if the rules in this policy match the given member info.
        /// </summary>
        /// <param name="member">MemberInfo to check against.</param>
        /// <returns>true if rule set matches, false if it does not.</returns>
        protected override bool DoesMatch(MethodImplementationInfo member)
        {
            bool matchesInterface = member.InterfaceMethodInfo != null ? Matches(member.InterfaceMethodInfo) : false;
            bool matchesImplementation = Matches(member.ImplementationMethodInfo);
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
                foreach (var callHandler in _handlers)
                {
                    yield return callHandler;
                }
            }
        }


        #region Implementation

        /// <summary>
        /// Tests the given member against the rule set. The member matches
        /// if all contained rules in the rule set match against it.
        /// </summary>
        /// <remarks>If the rule set is empty, then Matches passes since no rules failed.</remarks>
        /// <param name="member">MemberInfo to test.</param>
        /// <returns>true if all contained rules match, false if any fail.</returns>
        public bool Matches(MethodBase member)
        {
            if (_rules.Length == 0)
            {
                return false;
            }

            foreach (IMatchingRule rule in _rules)
            {
                if (!rule.Matches(member))
                {
                    return false;
                }
            }

            return true;
        }


        #endregion
    }
}
