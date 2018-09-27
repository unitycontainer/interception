

using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Interception.Utilities;

namespace Unity.Interception.PolicyInjection.MatchingRules
{
    /// <summary>
    /// A matching rule that matches when the member is declared
    /// in the given type.
    /// </summary>
    public class TypeMatchingRule : IMatchingRule
    {
        private readonly List<MatchingInfo> _matches;
        private readonly bool _matchesTypelessMembers;

        /// <summary>
        /// Constructs a new <see cref="TypeMatchingRule"/> that matches the
        /// given type.
        /// </summary>
        /// <param name="type">The type to match.</param>
        public TypeMatchingRule(Type type)
            : this(SafeGetTypeName(type), false)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="TypeMatchingRule"/> that matches types
        /// with the given name.
        /// </summary>
        /// <remarks>Comparisons are case sensitive.</remarks>
        /// <param name="typeName">Type name to match.</param>
        public TypeMatchingRule(string typeName)
            : this(typeName, false)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="TypeMatchingRule"/> that matches types
        /// with the given name, using the given case sensitivity.
        /// </summary>
        /// <param name="typeName">Type name to match.</param>
        /// <param name="ignoreCase">if false, do case-sensitive comparison. If true, do case-insensitive.</param>
        public TypeMatchingRule(string typeName, bool ignoreCase)
            : this(new[] { new MatchingInfo(typeName, ignoreCase) })
        {
        }

        /// <summary>
        /// Constructs a new <see cref="TypeMatchingRule"/> that will match
        /// any of the type names given in the collection of match information.
        /// </summary>
        /// <param name="matches">The match information to match.</param>
        public TypeMatchingRule(IEnumerable<MatchingInfo> matches)
        {
            _matches = new List<MatchingInfo>(matches);
            _matchesTypelessMembers = _matches.Exists(match => string.IsNullOrEmpty(
                match.Match));
        }

        /// <summary>
        /// Checks if the given member matches any of this object's matches.
        /// </summary>
        /// <param name="member">Member to match.</param>
        /// <returns>True if match, false if not.</returns>
        public bool Matches(MethodBase member)
        {
            Guard.ArgumentNotNull(member, "member");

            if (member.DeclaringType == null)
            {
                return _matchesTypelessMembers;
            }
            bool doesMatch = Matches(member.DeclaringType);
            return doesMatch;
        }

        /// <summary>
        /// Checks if the given type matches any of this object's matches.
        /// </summary>
        /// <remarks>Matches may be on the namespace-qualified type name or just the type name.</remarks>
        /// <param name="t">Type to check.</param>
        /// <returns>True if it matches, false if it doesn't.</returns>
        public bool Matches(Type t)
        {
            Guard.ArgumentNotNull(t, "t");

            foreach (MatchingInfo match in _matches)
            {
                if (string.Compare(t.FullName, match.Match, Comparison(match.IgnoreCase)) == 0)
                {
                    return true;
                }
                if (string.Compare(t.Name, match.Match, Comparison(match.IgnoreCase)) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static StringComparison Comparison(bool ignoreCase)
        {
            return ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        }

        private static string SafeGetTypeName(Type type)
        {
            Guard.ArgumentNotNull(type, "type");
            return type.Name;
        }
    }
}
