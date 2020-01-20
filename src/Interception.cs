using System;
using System.Globalization;
using Unity.Builder;
using Unity.Extension;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.ContainerIntegration.ObjectBuilder;
using Unity.Interception.Interceptors;
using Unity.Interception.Interceptors.InstanceInterceptors;
using Unity.Interception.Interceptors.TypeInterceptors;
using Unity.Interception.PolicyInjection;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.Properties;
using Unity.Interception.Utilities;

namespace Unity.Interception
{
    /// <summary>
    /// A Unity container extension that allows you to configure
    /// whether an object should be intercepted and which mechanism should
    /// be used to do it, and also provides a convenient set of methods for
    /// configuring injection for <see cref="RuleDrivenPolicy"/> instances.
    /// </summary>
    /// <seealso cref="SetDefaultInterceptorFor(Type, IInstanceInterceptor)"/>
    /// <seealso cref="SetDefaultInterceptorFor(Type, ITypeInterceptor)"/>
    /// <seealso cref="SetInterceptorFor(Type, string, IInstanceInterceptor)"/>
    /// <seealso cref="SetInterceptorFor(Type, string, ITypeInterceptor)"/>
    /// <seealso cref="AddPolicy"/>
    public class Interception : UnityContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        protected override void Initialize()
        {
            //Context.Strategies.Add(new InstanceInterceptionStrategy(),  UnityBuildStage.Lifetime);
            //Context.Strategies.Add(new TypeInterceptionStrategy(), UnityBuildStage.PreCreation);
            //Context.Container.RegisterInstance<InjectionPolicy>(typeof(AttributeDrivenPolicy).AssemblyQualifiedName,
            //                                                    new AttributeDrivenPolicy());
        }


        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <param name="typeToIntercept">Type to intercept.</param>
        /// <param name="name">Name type is registered under.</param>
        /// <param name="interceptor">Interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor(Type typeToIntercept, string name, ITypeInterceptor interceptor)
        {
            Guard.ArgumentNotNull(typeToIntercept, "typeToIntercept");
            Guard.ArgumentNotNull(interceptor, "interceptor");
            GuardTypeInterceptable(typeToIntercept, interceptor);

            var key = new NamedTypeBuildKey(typeToIntercept, name);

            var policy = new FixedTypeInterceptionPolicy(interceptor);
            Context.Policies.Set(key.Type, key.Name, typeof(ITypeInterceptionPolicy), policy);

            // add policy injection behavior if using this configuration API to set the interceptor
            var interceptionBehaviorsPolicy = new InterceptionBehaviorsPolicy();
            interceptionBehaviorsPolicy.AddBehaviorKey(NamedTypeBuildKey.Make<PolicyInjectionBehavior>());
            Context.Policies.Set(key.Type, key.Name, typeof(IInterceptionBehaviorsPolicy), interceptionBehaviorsPolicy);

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
        /// <typeparam name="T">Type to intercept</typeparam>
        /// <param name="name">Name type is registered under.</param>
        /// <param name="interceptor">Interceptor object to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor<T>(string name, ITypeInterceptor interceptor)
        {
            return SetInterceptorFor(typeof(T), name, interceptor);
        }

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <typeparam name="T">Type to intercept</typeparam>
        /// <param name="interceptor">Interceptor object to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor<T>(ITypeInterceptor interceptor)
        {
            return SetInterceptorFor(typeof(T), null, interceptor);
        }

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <param name="typeToIntercept">Type to intercept.</param>
        /// <param name="name">Name type is registered under.</param>
        /// <param name="interceptor">Instance interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor(Type typeToIntercept, string name, IInstanceInterceptor interceptor)
        {
            Guard.ArgumentNotNull(typeToIntercept, "typeToIntercept");
            Guard.ArgumentNotNull(interceptor, "interceptor");
            GuardTypeInterceptable(typeToIntercept, interceptor);

            var key = new NamedTypeBuildKey(typeToIntercept, name);

            var policy = new FixedInstanceInterceptionPolicy(interceptor);
            Context.Policies.Set(key.Type, key.Name, typeof(IInstanceInterceptionPolicy), policy);

            // add policy injection behavior if using this configuration API to set the interceptor
            var interceptionBehaviorsPolicy = new InterceptionBehaviorsPolicy();
            interceptionBehaviorsPolicy.AddBehaviorKey(NamedTypeBuildKey.Make<PolicyInjectionBehavior>());
            Context.Policies.Set(key.Type, key.Name, typeof(IInterceptionBehaviorsPolicy), interceptionBehaviorsPolicy);

            return this;
        }

        /// <summary>
        /// Set the interceptor for a type, regardless of what name is used to resolve the instances.
        /// </summary>
        /// <param name="typeToIntercept">Type to intercept</param>
        /// <param name="interceptor">Interceptor instance.</param>
        /// <returns>This extension object.</returns>
        public Interception SetDefaultInterceptorFor(Type typeToIntercept, ITypeInterceptor interceptor)
        {
            throw new NotImplementedException();

            //Guard.ArgumentNotNull(typeToIntercept, "typeToIntercept");
            //Guard.ArgumentNotNull(interceptor, "interceptor");
            //GuardTypeInterceptable(typeToIntercept, interceptor);

            //Context.Policies.Set(typeToIntercept, UnityContainer.All, typeof(ITypeInterceptionPolicy), 
            //                     new FixedTypeInterceptionPolicy(interceptor));

            //// add policy injection behavior if using this configuration API to set the interceptor
            //var interceptionBehaviorsPolicy = new InterceptionBehaviorsPolicy();
            //interceptionBehaviorsPolicy.AddBehaviorKey(NamedTypeBuildKey.Make<PolicyInjectionBehavior>());
            //Context.Policies.Set(typeToIntercept, UnityContainer.All, 
            //                    typeof(IInterceptionBehaviorsPolicy), interceptionBehaviorsPolicy);
            //return this;
        }

        /// <summary>
        /// Set the interceptor for a type, regardless of what name is used to resolve the instances.
        /// </summary>
        /// <typeparam name="TTypeToIntercept">Type to intercept</typeparam>
        /// <param name="interceptor">Interceptor instance.</param>
        /// <returns>This extension object.</returns>
        public Interception SetDefaultInterceptorFor<TTypeToIntercept>(ITypeInterceptor interceptor)
        {
            return SetDefaultInterceptorFor(typeof(TTypeToIntercept), interceptor);
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

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <typeparam name="T">Type to intercept.</typeparam>
        /// <param name="name">Name type is registered under.</param>
        /// <param name="interceptor">Instance interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor<T>(string name, IInstanceInterceptor interceptor)
        {
            return SetInterceptorFor(typeof(T), name, interceptor);
        }

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <typeparam name="T">Type to intercept.</typeparam>
        /// <param name="interceptor">Instance interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public Interception SetInterceptorFor<T>(IInstanceInterceptor interceptor)
        {
            return SetInterceptorFor(typeof(T), null, interceptor);
        }

        /// <summary>
        /// API to configure the default interception settings for a type.
        /// </summary>
        /// <param name="typeToIntercept">Type the interception is being configured for.</param>
        /// <param name="interceptor">The interceptor to use by default.</param>
        /// <returns>This extension object.</returns>
        public Interception SetDefaultInterceptorFor(Type typeToIntercept, IInstanceInterceptor interceptor)
        {
            throw new NotImplementedException();

            //Guard.ArgumentNotNull(typeToIntercept, "typeToIntercept");
            //Guard.ArgumentNotNull(interceptor, "interceptor");
            //GuardTypeInterceptable(typeToIntercept, interceptor);

            //Context.Policies.Set(typeToIntercept, UnityContainer.All, typeof(IInstanceInterceptionPolicy), new FixedInstanceInterceptionPolicy(interceptor));

            //// add policy injection behavior if using this configuration API to set the interceptor
            //var interceptionBehaviorsPolicy = new InterceptionBehaviorsPolicy();
            //interceptionBehaviorsPolicy.AddBehaviorKey(NamedTypeBuildKey.Make<PolicyInjectionBehavior>());
            //Context.Policies.Set(typeToIntercept, UnityContainer.All, typeof(IInterceptionBehaviorsPolicy), interceptionBehaviorsPolicy);

            //return this;
        }

        /// <summary>
        /// API to configure the default interception settings for a type.
        /// </summary>
        /// <typeparam name="TTypeToIntercept">Type the interception is being configured for.</typeparam>
        /// <param name="interceptor">The interceptor to use by default.</param>
        /// <returns>This extension object.</returns>
        public Interception SetDefaultInterceptorFor<TTypeToIntercept>(IInstanceInterceptor interceptor)
        {
            return SetDefaultInterceptorFor(typeof(TTypeToIntercept), interceptor);
        }

        private static void GuardTypeInterceptable(Type typeToIntercept, IInterceptor interceptor)
        {
            if (!interceptor.CanIntercept(typeToIntercept))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.InterceptionNotSupported,
                        typeToIntercept.FullName),
                    nameof(typeToIntercept));
            }
        }

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
            Guard.ArgumentNotNullOrEmpty(policyName, "policyName");
            return new PolicyDefinition(policyName, this);
        }
    }
}
