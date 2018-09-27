

using System;
using System.Reflection;
using Unity.Interception.Utilities;

namespace Unity.Interception.PolicyInjection.MatchingRules
{
    /// <summary>
    /// A <see cref="TagAttribute"/> that checks a member for the presence
    /// of the <see cref="IMatchingRule"/> on the method, property, or class, and
    /// that the given string matches.
    /// </summary>
    public class TagAttributeMatchingRule : IMatchingRule
    {
        private readonly string _tagToMatch;
        private readonly bool _ignoreCase;

        /// <summary>
        /// Constructs a new <see cref="TagAttributeMatchingRule"/>, looking for
        /// the given string. The comparison is case sensitive.
        /// </summary>
        /// <param name="tagToMatch">tag string to match.</param>
        public TagAttributeMatchingRule(string tagToMatch)
            : this(tagToMatch, false)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="TagAttributeMatchingRule"/>, looking for
        /// the given string. The comparison is case sensitive if <paramref name="ignoreCase"/> is
        /// false, case insensitive if <paramref name="ignoreCase"/> is true.
        /// </summary>
        /// <param name="tagToMatch">tag string to match.</param>
        /// <param name="ignoreCase">if false, case-sensitive comparison. If true, case-insensitive comparison.</param>
        public TagAttributeMatchingRule(string tagToMatch, bool ignoreCase)
        {
            _tagToMatch = tagToMatch;
            _ignoreCase = ignoreCase;
        }

        /// <summary>
        /// Check the given member for the presence of the <see cref="TagAttribute"/> and
        /// match the strings.
        /// </summary>
        /// <param name="member">Member to check.</param>
        /// <returns>True if tag strings match, false if they don't.</returns>
        public bool Matches(MethodBase member)
        {
            foreach (TagAttribute tagAttribute in ReflectionHelper.GetAllAttributes<TagAttribute>(member, true))
            {
                if (string.Compare(tagAttribute.Tag, _tagToMatch, _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
