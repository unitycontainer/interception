

using System.Collections.Generic;
using System.Reflection;
using Unity.Interception.Utilities;

namespace Unity.Interception.PolicyInjection.MatchingRules
{
    /// <summary>
    /// Match methods with the given names and method signature.
    /// </summary>
    public class MethodSignatureMatchingRule : IMatchingRule
    {
        private readonly Glob _methodNamePattern;
        private readonly List<TypeMatchingRule> _parameterRules;

        /// <summary>
        /// Creates a new <see cref="MethodSignatureMatchingRule"/> that matches methods
        /// with the given name, with parameter types matching the given list.
        /// </summary>
        /// <param name="methodName">Method name to match. Wildcards are allowed.</param>
        /// <param name="parameterTypeNames">Parameter type names to match, in order. Wildcards are allowed.</param>
        /// <param name="ignoreCase">If false, name comparisons are case sensitive. If true, name comparisons are case insensitive.</param>
        public MethodSignatureMatchingRule(string methodName, IEnumerable<string> parameterTypeNames, bool ignoreCase)
        {
            Guard.ArgumentNotNull(parameterTypeNames, "parameterTypeNames");

            _methodNamePattern = new Glob(methodName, !ignoreCase);
            _parameterRules = new List<TypeMatchingRule>();

            foreach (string parameterTypeName in parameterTypeNames)
            {
                _parameterRules.Add(new TypeMatchingRule(parameterTypeName, ignoreCase));
            }
        }

        /// <summary>
        /// Create a new <see cref="MethodSignatureMatchingRule"/> that matches methods
        /// with the given name, with parameter types matching the given list.
        /// </summary>
        /// <remarks>Name comparisons are case sensitive.</remarks>
        /// <param name="methodName">Method name to match. Wildcards are allowed.</param>
        /// <param name="parameterTypeNames">Parameter type names to match, in order. Wildcards are allowed.</param>
        public MethodSignatureMatchingRule(string methodName, IEnumerable<string> parameterTypeNames)
            : this(methodName, parameterTypeNames, false)
        {
        }

        /// <summary>
        /// Create a new <see cref="MethodSignatureMatchingRule"/> that matches any method
        /// with parameter types matching the given list.
        /// </summary>
        /// <remarks>Name comparisons are case sensitive.</remarks>
        /// <param name="parameterTypeNames">Parameter type names to match, in order. Wildcards are allowed.</param>
        public MethodSignatureMatchingRule(IEnumerable<string> parameterTypeNames)
            : this("*", parameterTypeNames, false)
        {
        }

        /// <summary>
        /// Create a new <see cref="MethodSignatureMatchingRule"/> that matches any method
        /// with parameter types matching the given list.
        /// </summary>
        /// <param name="parameterTypeNames">Parameter type names to match, in order. Wildcards are allowed.</param>
        /// <param name="ignoreCase">If false, name comparisons are case sensitive. If true, name comparisons are case insensitive.</param>
        public MethodSignatureMatchingRule(IEnumerable<string> parameterTypeNames, bool ignoreCase)
            : this("*", parameterTypeNames, ignoreCase)
        {
        }

        /// <summary>
        /// Check to see if the given method matches the name and signature.
        /// </summary>
        /// <param name="member">Member to check.</param>
        /// <returns>True if match, false if not.</returns>
        public bool Matches(MethodBase member)
        {
            Guard.ArgumentNotNull(member, "member");

            if (!_methodNamePattern.IsMatch(member.Name))
            {
                return false;
            }

            ParameterInfo[] parameters = member.GetParameters();
            if (parameters.Length != _parameterRules.Count)
            {
                return false;
            }

            for (int i = 0; i < parameters.Length; ++i)
            {
                if (!_parameterRules[i].Matches(parameters[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
