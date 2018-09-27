

using System;
using System.Reflection;
using Unity.Interception.PolicyInjection.MatchingRules;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    public class TypeMatchingAssignmentRule : IMatchingRule
    {
        private Type matchType;

        public TypeMatchingAssignmentRule(Type matchType)
        {
            this.matchType = matchType;
        }

        public bool Matches(MethodBase member)
        {
            //if(member is Type)
            //{
            //    return member == matchType;
            //}

            return (member.DeclaringType == matchType);
        }
    }
}
