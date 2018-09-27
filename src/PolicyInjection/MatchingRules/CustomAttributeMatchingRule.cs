

using System;
using System.Reflection;
using Unity.Interception.Properties;
using Unity.Interception.Utilities;

namespace Unity.Interception.PolicyInjection.MatchingRules
{
    /// <summary>
    /// An implementation of <see cref="IMatchingRule"/> that checks to see if
    /// the member tested has an arbitrary attribute applied.
    /// </summary>
    public class CustomAttributeMatchingRule : IMatchingRule
    {
        private readonly Type _attributeType;
        private readonly bool _inherited;

        /// <summary>
        /// Constructs a new <see cref="CustomAttributeMatchingRule"/>.
        /// </summary>
        /// <param name="attributeType">Attribute to match.</param>
        /// <param name="inherited">If true, checks the base class for attributes as well.</param>
        public CustomAttributeMatchingRule(Type attributeType, bool inherited)
        {
            Guard.ArgumentNotNull(attributeType, "attributeType");
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
            {
                throw new ArgumentException(Resources.ExceptionAttributeNoSubclassOfAttribute, nameof(attributeType));
            }

            _attributeType = attributeType;
            _inherited = inherited;
        }

        /// <summary>
        /// Checks to see if the given <paramref name="member"/> matches the rule.
        /// </summary>
        /// <param name="member">Member to check.</param>
        /// <returns>true if it matches, false if not.</returns>
        public bool Matches(MethodBase member)
        {
            Guard.ArgumentNotNull(member, "member");

            object[] attribues = member.GetCustomAttributes(_attributeType, _inherited);

            return (attribues != null && attribues.Length > 0);
        }
    }
}
