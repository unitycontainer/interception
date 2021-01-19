using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Interception.Interceptors;
using Unity.Interception.PolicyInjection.MatchingRules;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Unity.Interception.PolicyInjection.Policies
{
    /// <summary>
    /// Base class for Policies that specifies which handlers apply to which methods of an object.
    /// </summary>
    /// <remarks>
    /// <para>This base class always enforces the 
    /// <see cref="ApplyNoPoliciesMatchingRule"/> before
    /// passing the checks onto derived classes. This way, derived classes do not need to
    /// worry about implementing this check.</para>
    /// <para>It also means that derived classes cannot override this rule. This is considered a feature.</para></remarks>
    public abstract class InjectionPolicy
    {
        #region Fields

        private readonly string        _name;
        private readonly IMatchingRule _rule;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new empty Policy.
        /// </summary>
        protected InjectionPolicy()
            : this("Unnamed policy")
        {
        }

        /// <summary>
        /// Creates a new empty policy with the given name.
        /// </summary>
        /// <param name="name">Name of the policy.</param>
        protected InjectionPolicy(string name)
        {
            _name = name;
            _rule = new ApplyNoPoliciesMatchingRule();
        }

        #endregion


        #region Public Members

        /// <summary>
        /// Gets the name of this policy.
        /// </summary>
        /// <value>The name of the policy.</value>
        public string Name => _name;

        /// <summary>
        /// Checks if the rules in this policy match the given member info.
        /// </summary>
        /// <param name="member">MemberInfo to check against.</param>
        /// <returns>true if rule set matches, false if it does not.</returns>
        public bool Matches(MethodImplementationInfo member) 
            => DoesNotHaveNoPoliciesAttributeRule(member) && DoesMatch(member);

        /// <summary>
        /// Returns ordered collection of handlers in order that apply to the given member.
        /// </summary>
        /// <param name="member">Member that may or may not be assigned handlers by this policy.</param>
        /// <param name="container">The <see cref="IUnityContainer"/> to use when creating handlers,
        /// if necessary.</param>
        /// <returns>Collection of handlers (possibly empty) that apply to this member.</returns>
        public virtual IEnumerable<ICallHandler> GetHandlersFor(MethodImplementationInfo member, IUnityContainer container) 
            => DoesNotHaveNoPoliciesAttributeRule(member)
                ? DoGetHandlersFor(member, container)
                : Enumerable.Empty<ICallHandler>();

        #endregion


        #region Implementation

        private bool DoesNotHaveNoPoliciesAttributeRule(MethodImplementationInfo method)
        {
            var doesNotHaveRule = true;
            doesNotHaveRule &= method.InterfaceMethodInfo != null ? _rule.Matches(method.InterfaceMethodInfo) : true;
            doesNotHaveRule &= _rule.Matches(method.ImplementationMethodInfo);
            return doesNotHaveRule;
        }


        /// <summary>
        /// Given a method on an object, return the set of MethodBases for that method,
        /// plus any interface methods that the member implements.
        /// </summary>
        /// <param name="member">Member to get Method Set for.</param>
        /// <returns>The set of methods</returns>
        protected static IEnumerable<MethodBase> GetMethodSet(MethodBase member)
        {
            List<MethodBase> methodSet = new List<MethodBase>(new[] { member });
            if (member.DeclaringType != null && !member.DeclaringType.IsInterface)
            {
                foreach (Type itf in member.DeclaringType.GetInterfaces())
                {
                    InterfaceMapping mapping = member.DeclaringType.GetInterfaceMap(itf);
                    for (int i = 0; i < mapping.TargetMethods.Length; ++i)
                    {
                        if (mapping.TargetMethods[i] == member)
                        {
                            methodSet.Add(mapping.InterfaceMethods[i]);
                        }
                    }
                }
            }

            return methodSet;
        }


        /// <summary>
        /// Derived classes implement this method to calculate if the policy
        /// will provide any handler to the specified member.
        /// </summary>
        /// <param name="member">Member to check.</param>
        /// <returns>true if policy applies to this member, false if not.</returns>
        protected abstract bool DoesMatch(MethodImplementationInfo member);

        /// <summary>
        /// Derived classes implement this method to supply the list of handlers for
        /// this specific member.
        /// </summary>
        /// <param name="member">Member to get handlers for.</param>
        /// <param name="container">The <see cref="IUnityContainer"/> to use when creating handlers,
        /// if necessary.</param>
        /// <returns>Enumerable collection of handlers for this method.</returns>
        protected abstract IEnumerable<ICallHandler> DoGetHandlersFor(MethodImplementationInfo member, IUnityContainer container);
        
        #endregion
    }
}
