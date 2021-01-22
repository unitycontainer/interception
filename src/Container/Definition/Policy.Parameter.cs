using System;
using System.Collections;
using Unity.Injection;
using Unity.Interception.PolicyInjection.MatchingRules;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Unity.Interception
{
    public partial class PolicyDefinition : ParameterValue
    {
        protected override MatchRank Match(Type type) 
            => ReferenceEquals(typeof(IMatchingRule[]), type) || 
               ReferenceEquals(typeof(ICallHandler[]),  type)
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch;


        public override void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
            => descriptor.Arguments 
            = ReferenceEquals(typeof(IMatchingRule[]), descriptor.ContractType)
            ? (IList)_rules
            : (IList)_handlers;
    }
}
