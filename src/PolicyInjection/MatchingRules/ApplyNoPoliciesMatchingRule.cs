

using System.Reflection;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.Utilities;

namespace Unity.Interception.PolicyInjection.MatchingRules
{
    /// <summary>
    /// A <see cref="IMatchingRule"/> implementation that fails to match
    /// if the method in question has the ApplyNoPolicies attribute on it.
    /// </summary>
    internal class ApplyNoPoliciesMatchingRule : IMatchingRule
    {
        /// <summary>
        /// Check if the <paramref name="member"/> matches this rule.
        /// </summary>
        /// <remarks>This rule returns true if the member does NOT have the <see cref="ApplyNoPoliciesAttribute"/>
        /// on it, or a containing type doesn't have the attribute.</remarks>
        /// <param name="member">Member to check.</param>
        /// <returns>True if the rule matches, false if it doesn't.</returns>
        public bool Matches(MethodBase member)
        {
            Guard.ArgumentNotNull(member, "member");

            bool hasNoPoliciesAttribute =
                (member.GetCustomAttributes(typeof(ApplyNoPoliciesAttribute), false).Length != 0);

            hasNoPoliciesAttribute |=
                (member.DeclaringType.GetCustomAttributes(typeof(ApplyNoPoliciesAttribute), false).
                    Length != 0);
            return !hasNoPoliciesAttribute;
        }
    }
}
