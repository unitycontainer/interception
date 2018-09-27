

using System;

namespace Unity.Interception.PolicyInjection.MatchingRules
{
    /// <summary>
    /// A simple attribute used to "tag" classes, methods, or properties with a
    /// string that can later be matched via the <see cref="TagAttributeMatchingRule"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class TagAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="TagAttribute"/> with the given string.
        /// </summary>
        /// <param name="tag">The tag string.</param>
        public TagAttribute(string tag)
        {
            Tag = tag;
        }

        /// <summary>
        /// The string tag for this attribute.
        /// </summary>
        /// <value>the tag.</value>
        public string Tag { get; }
    }
}
