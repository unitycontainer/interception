using System;
using System.Collections.Generic;
using Unity.Injection;
using Unity.Interception.PolicyInjection.MatchingRules;
using Unity.Interception.PolicyInjection.Policies;

namespace Unity.Interception
{
    /// <summary>
    /// Transient class that supports convenience method for specifying interception policies.
    /// </summary>
    public partial class PolicyDefinition
    {
        #region Fields

        private readonly string       _name;
        private readonly Interception _extension;
        private readonly List<ResolvedParameter> _rulesParameters;
        private readonly List<string> _handlersNames;

        #endregion

        #region Constructors

        internal PolicyDefinition(string? policyName, Interception extension)
        {
            _name = policyName ?? Guid.NewGuid().ToString(); 
            _extension = extension;

            _rulesParameters = new List<ResolvedParameter>();
            _handlersNames = new List<string>();

            UpdateRuleDrivenPolicyInjection();
        }

        #endregion


        private PolicyDefinition UpdateRuleDrivenPolicyInjection()
        {
            _extension.Container
                .RegisterType<InjectionPolicy, RuleDrivenPolicy>(_name,
                    new InjectionConstructor(_name,
                        new ResolvedArrayParameter<IMatchingRule>(_rulesParameters.ToArray()), _handlersNames.ToArray()));
            return this;
        }

        private delegate PolicyDefinition UpdateElements(string name);

        private PolicyDefinition UpdateRulesParameters(string name)
        {
            _rulesParameters.Add(new ResolvedParameter<IMatchingRule>(name));

            return UpdateRuleDrivenPolicyInjection();
        }

        private PolicyDefinition UpdateHandlerNames(string name)
        {
            _handlersNames.Add(name);

            return UpdateRuleDrivenPolicyInjection();
        }

        private static string NewName()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// The <see cref="IUnityContainer"/> that is currently being
        /// configured.
        /// </summary>
        public IUnityContainer Container => _extension.Container;

        /// <summary>
        /// The <see cref="Interception"/> extension to which the policy was added.
        /// </summary>
        /// <remarks>
        /// Use this property to start adding a new policy.
        /// </remarks>
        public Interception Interception => _extension;
    }
}
