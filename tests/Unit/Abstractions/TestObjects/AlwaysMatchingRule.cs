using System.Reflection;
using Unity.Interception.PolicyInjection.MatchingRules;

namespace Unity.Interception.Tests
{
    /// <summary>
    /// A simple matching rule class that always matches. Useful when you want
    /// a policy to apply across the board.
    /// </summary>
    public class AlwaysMatchingRule : IMatchingRule
    {
        [InjectionConstructor]
        public AlwaysMatchingRule()
        {
        }

        public bool Matches(MethodBase member)
        {
            return true;
        }
    }
}
