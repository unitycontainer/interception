using System;
using Unity.Extension;
using Unity.Injection;
using Unity.Interception.Utilities;

namespace Unity.Interception
{
    public partial class Interception : IUnityContainerExtensionConfigurator
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
            //Guard.ArgumentNotNull(typeToIntercept, "typeToIntercept");
            //Guard.ArgumentNotNull(interceptor, "interceptor");
            //GuardTypeInterceptable(typeToIntercept, interceptor);

            //var key = new Contract(typeToIntercept, name);

            //var policy = new FixedTypeInterceptionPolicy(interceptor);
            //Context.Policies.Set(key.Type, key.Name, typeof(ITypeInterceptionPolicy), policy);

            //// add policy injection behavior if using this configuration API to set the interceptor
            //var interceptionBehaviorsPolicy = new InterceptionBehaviorsPolicy();
            //interceptionBehaviorsPolicy.AddBehaviorKey(NamedTypeBuildKey.Make<PolicyInjectionBehavior>());
            //Context.Policies.Set(key.Type, key.Name, typeof(IInterceptionBehaviorsPolicy), interceptionBehaviorsPolicy);

            return this;
        }

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <param name="typeToIntercept">Type to intercept.</param>
        /// <param name="interceptor">Interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor(Type typeToIntercept, ITypeInterceptor interceptor)
        {
            return SetInterceptorFor(typeToIntercept, null, interceptor);
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
            Guard.ArgumentNotNull(typeToIntercept, "typeToIntercept");
            Guard.ArgumentNotNull(interceptor, "interceptor");

            if (!interceptor.CanIntercept(typeToIntercept))
                throw new ArgumentException($"The type {typeToIntercept.FullName} is not interceptable.");

            //var key = new Contract(typeToIntercept, name);

            //var policy = new FixedInstanceInterceptionPolicy(interceptor);
            //Context.Policies.Set(key.Type, key.Name, typeof(IInstanceInterceptionPolicy), policy);

            //// add policy injection behavior if using this configuration API to set the interceptor
            //var interceptionBehaviorsPolicy = new InterceptionBehaviorsPolicy();
            //interceptionBehaviorsPolicy.AddBehaviorKey(NamedTypeBuildKey.Make<PolicyInjectionBehavior>());
            //Context.Policies.Set(key.Type, key.Name, typeof(IInterceptionBehaviorsPolicy), interceptionBehaviorsPolicy);

            return this;
        }

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <param name="typeToIntercept">Type to intercept.</param>
        /// <param name="interceptor">Instance interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor(Type typeToIntercept, IInstanceInterceptor interceptor)
        {
            return SetInterceptorFor(typeToIntercept, null, interceptor);
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
            // TODO: Validation
            //Guard.ArgumentNotNull(typeToIntercept, "typeToIntercept");
            //Guard.ArgumentNotNull(interceptor, "interceptor");
            //GuardTypeInterceptable(typeToIntercept, interceptor);

            Context!.Policies.Set<ITypeInterceptor>(typeToIntercept, interceptor);

            //// add policy injection behavior if using this configuration API to set the interceptor
            //var interceptionBehaviorsPolicy = new InterceptionBehaviorsPolicy();
            //interceptionBehaviorsPolicy.AddBehaviorKey(NamedTypeBuildKey.Make<PolicyInjectionBehavior>());
            //Context.Policies.Set(typeToIntercept, UnityContainer.All, 
            //                    typeof(IInterceptionBehaviorsPolicy), interceptionBehaviorsPolicy);
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
            // TODO: Validation
            //Guard.ArgumentNotNull(typeToIntercept, "typeToIntercept");
            //Guard.ArgumentNotNull(interceptor, "interceptor");
            //GuardTypeInterceptable(typeToIntercept, interceptor);

            Context!.Policies.Set<IInstanceInterceptor>(typeToIntercept, interceptor);

            // add policy injection behavior if using this configuration API to set the interceptor
            //var interceptionBehaviorsPolicy = new InterceptionBehaviorsPolicy();
            //interceptionBehaviorsPolicy.AddBehaviorKey(NamedTypeBuildKey.Make<PolicyInjectionBehavior>());
            //Context.Policies.Set(typeToIntercept, UnityContainer.All, typeof(IInterceptionBehaviorsPolicy), interceptionBehaviorsPolicy);

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

            // TODO: add support for updated lifetime
            Container!.RegisterType<InjectionPolicy, RuleDrivenPolicy>(policy.Name, 
                new InjectionConstructor(policy.Name, policy, policy));

            return policy;
        }

        #endregion
    }
}
