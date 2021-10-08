using System;
using Unity.Injection;
using Unity.Interception.ContainerIntegration;

namespace Unity.Interception
{
    public partial class Interception
    {
        #region Set

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <param name="typeToIntercept">Type to intercept.</param>
        /// <param name="name">Name type is registered under.</param>
        /// <param name="interceptor">Interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor(Type typeToIntercept, string? name, ITypeInterceptor interceptor)
        {
            Set(typeToIntercept, name, interceptor);

            this.Set<IInterceptionBehaviorsPolicy>(typeToIntercept, name, 
                new InterceptionBehaviorsPolicy(typeof(PolicyInjectionBehavior)));

            return this;
        }

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <param name="typeToIntercept">Type to intercept.</param>
        /// <param name="name">Name type is registered under.</param>
        /// <param name="interceptor">Instance interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor(Type typeToIntercept, string? name, IInstanceInterceptor interceptor)
        {
            Set(typeToIntercept, name, typeof(IInstanceInterceptor), interceptor);

            this.Set<IInterceptionBehaviorsPolicy>(typeToIntercept, name,
                new InterceptionBehaviorsPolicy(typeof(PolicyInjectionBehavior)));

            return this;
        }

        #endregion


        #region Set Default

        /// <summary>
        /// Set the interceptor for a type, regardless of what name is used to resolve the instances.
        /// </summary>
        /// <param name="typeToIntercept">Type to intercept</param>
        /// <param name="interceptor">Interceptor instance.</param>
        /// <returns>This extension object.</returns>
        public Interception SetDefaultInterceptorFor(Type typeToIntercept, ITypeInterceptor interceptor)
        {
            Set(typeToIntercept, typeof(ITypeInterceptor), interceptor);

            this.Set<IInterceptionBehaviorsPolicy>(typeToIntercept,
                new InterceptionBehaviorsPolicy(typeof(PolicyInjectionBehavior)));

            return this;
        }

        /// <summary>
        /// API to configure the default interception settings for a type.
        /// </summary>
        /// <param name="typeToIntercept">Type the interception is being configured for.</param>
        /// <param name="interceptor">The interceptor to use by default.</param>
        /// <returns>This extension object.</returns>
        public Interception SetDefaultInterceptorFor(Type typeToIntercept, IInstanceInterceptor interceptor)
        {
            Set(typeToIntercept, typeof(IInstanceInterceptor), interceptor);

            this.Set<IInterceptionBehaviorsPolicy>(typeToIntercept,
                new InterceptionBehaviorsPolicy(typeof(PolicyInjectionBehavior)));

            return this;
        }

        #endregion


        #region Policy

        /// <summary>
        /// Starts the definition of a new <see cref="RuleDrivenPolicy"/>.
        /// </summary>
        /// <param name="policyName">The policy name.</param>
        /// <returns></returns>
        /// <remarks>This is a convenient way for defining a new policy and the <see cref="IMatchingRule"/>
        /// instances and <see cref="ICallHandler"/> instances that are required by a policy.
        /// <para/>
        /// This mechanism is just a shortcut for what can be natively expressed by wiring up together objects
        /// with repeated calls to the <see cref="IUnityContainer.RegisterType"/> method.
        /// </remarks>
        public PolicyDefinition AddPolicy(string policyName)
        {
            var policy = new PolicyDefinition(policyName, this);

            Container!.RegisterType<InjectionPolicy, RuleDrivenPolicy>(policy.Name, new InjectionConstructor(policy.Name, policy, policy));

            return policy;
        }

        #endregion
    }
}
