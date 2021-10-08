using System;
using System.Collections;
using Unity.Injection;

namespace Unity.Interception
{
    public partial class PolicyDefinition : ParameterValue
    {
        protected override MatchRank Match(Type type) 
            => ReferenceEquals(typeof(IMatchingRule[]), type) || 
               ReferenceEquals(typeof(IList),  type)
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch;


        public override void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
        {
            if (ReferenceEquals(typeof(IMatchingRule[]), descriptor.ContractType))
                descriptor.Arguments = (IList)_rules;
            else
                descriptor.Value = (IList)_handlers;
        }
    }
}
