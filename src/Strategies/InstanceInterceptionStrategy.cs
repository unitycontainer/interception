using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Extension;
using Unity.Interception.Extensions;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors;
using Unity.Interception.Interceptors.InstanceInterceptors;

namespace Unity.Interception.ContainerIntegration.ObjectBuilder
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that intercepts objects
    /// in the build chain by creating a proxy object.
    /// </summary>
    public class InstanceInterceptionStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PostBuildUp<TContext>(ref TContext context)
        {
            // If it's already been intercepted, don't do it again.
            if (context.Target is IInterceptingProxy) return;

            IInstanceInterceptor interceptor =
                typeof(IInstanceInterceptor).FindInjectedMember<TContext, Interceptor>(ref context)?
                                            .GetInterceptor<TContext, IInstanceInterceptor>(ref context);
            if (null == interceptor)
            { 
                IInstanceInterceptionPolicy interceptionPolicy =
                    FindInterceptionPolicy<TContext, IInstanceInterceptionPolicy>(ref context, true);
                if (interceptionPolicy == null)
                {
                    return;
                }

                interceptor = interceptionPolicy.GetInterceptor(ref context);
            }

            IInterceptionBehaviorsPolicy interceptionBehaviorsPolicy =
                FindInterceptionPolicy<TContext, IInterceptionBehaviorsPolicy>(ref context, true);
            if (interceptionBehaviorsPolicy == null)
            {
                return;
            }

            IAdditionalInterfacesPolicy additionalInterfacesPolicy =
                FindInterceptionPolicy<TContext, IAdditionalInterfacesPolicy>(ref context, false);
            IEnumerable<Type> additionalInterfaces =
                additionalInterfacesPolicy != null ? additionalInterfacesPolicy.AdditionalInterfaces : Type.EmptyTypes;

            Type typeToIntercept = context.Type; // TODO: context.RegistrationType;
            Type implementationType = context.Target.GetType();

            IInterceptionBehavior[] interceptionBehaviors =
                interceptionBehaviorsPolicy.GetEffectiveBehaviors(
                    ref context, interceptor, typeToIntercept, implementationType)
                .ToArray();

            if (interceptionBehaviors.Length > 0)
            {
                context.Target =
                    Intercept.ThroughProxyWithAdditionalInterfaces(
                        typeToIntercept,
                        context.Target,
                        interceptor,
                        interceptionBehaviors,
                        additionalInterfaces);
            }
        }

        private static TPolicy FindInterceptionPolicy<TContext, TPolicy>(ref TContext context, bool probeOriginalKey)
            where TContext : IBuilderContext
            where TPolicy  : class
        {
            // First, try for an original build key
            var policy = GetPolicyOrDefault<TContext, TPolicy>(ref context);

            if (policy != null) return policy;

            if (!probeOriginalKey) return null;

            // Next, try the build type
            policy = GetPolicyOrDefault<TContext, TPolicy>(ref context);

            return policy;
        }

        public static TPolicyInterface GetPolicyOrDefault<TContext, TPolicyInterface>(ref TContext context)
            where TContext : IBuilderContext
        {
            throw new NotImplementedException();
//            return (TPolicyInterface)(GetNamedPolicy(ref context, context.RegistrationType, context.Name) ??
//                                      GetNamedPolicy(ref context, context.RegistrationType, UnityContainer.All));

//            object GetNamedPolicy(ref TContext c, Type t, string n)
//            {
//                return (c.Get(t, n, typeof(TPolicyInterface)) ?? (
//#if NETCOREAPP1_0 || NETSTANDARD1_0
//                            t.GetTypeInfo().IsGenericType
//#else
//                                    t.IsGenericType
//#endif
//                                        ? c.Get(t.GetGenericTypeDefinition(), n, typeof(TPolicyInterface)) ?? c.Get(null, null, typeof(TPolicyInterface))
//                                : c.Get(null, null, typeof(TPolicyInterface))));
//            }
        }
    }
}
