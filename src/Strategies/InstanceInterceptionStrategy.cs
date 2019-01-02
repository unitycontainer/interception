using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Builder;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors;
using Unity.Strategies;

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
        public override void PostBuildUp(ref BuilderContext context)
        {
            // If it's already been intercepted, don't do it again.
            if (context.Existing is IInterceptingProxy)
            {
                return;
            }

            IInstanceInterceptionPolicy interceptionPolicy =
                FindInterceptionPolicy<IInstanceInterceptionPolicy>(ref context, true);
            if (interceptionPolicy == null)
            {
                return;
            }

            var interceptor = interceptionPolicy.GetInterceptor(ref context);

            IInterceptionBehaviorsPolicy interceptionBehaviorsPolicy =
                FindInterceptionPolicy<IInterceptionBehaviorsPolicy>(ref context, true);
            if (interceptionBehaviorsPolicy == null)
            {
                return;
            }

            IAdditionalInterfacesPolicy additionalInterfacesPolicy =
                FindInterceptionPolicy<IAdditionalInterfacesPolicy>(ref context, false);
            IEnumerable<Type> additionalInterfaces =
                additionalInterfacesPolicy != null ? additionalInterfacesPolicy.AdditionalInterfaces : Type.EmptyTypes;

            Type typeToIntercept = context.RegistrationType;
            Type implementationType = context.Existing.GetType();

            IInterceptionBehavior[] interceptionBehaviors =
                interceptionBehaviorsPolicy.GetEffectiveBehaviors(
                    ref context, interceptor, typeToIntercept, implementationType)
                .ToArray();

            if (interceptionBehaviors.Length > 0)
            {
                context.Existing =
                    Intercept.ThroughProxyWithAdditionalInterfaces(
                        typeToIntercept,
                        context.Existing,
                        interceptor,
                        interceptionBehaviors,
                        additionalInterfaces);
            }
        }

        private static T FindInterceptionPolicy<T>(ref BuilderContext context, bool probeOriginalKey)
            where T : class
        {
            // First, try for an original build key
            var policy = GetPolicyOrDefault<T>(ref context);

            if (policy != null) return policy;

            if (!probeOriginalKey) return null;

            // Next, try the build type
            policy = GetPolicyOrDefault<T>(ref context);

            return policy;
        }

        public static TPolicyInterface GetPolicyOrDefault<TPolicyInterface>(ref BuilderContext context)
        {
            return (TPolicyInterface)(GetNamedPolicy(ref context, context.RegistrationType, context.Name) ??
                                      GetNamedPolicy(ref context, context.RegistrationType, UnityContainer.All));

            object GetNamedPolicy(ref BuilderContext c, Type t, string n)
            {
                return (c.Get(t, n, typeof(TPolicyInterface)) ?? (
#if NETCOREAPP1_0 || NETSTANDARD1_0
                            t.GetTypeInfo().IsGenericType
#else
                                    t.IsGenericType
#endif
                                        ? c.Get(t.GetGenericTypeDefinition(), n, typeof(TPolicyInterface)) ?? c.Get(null, null, typeof(TPolicyInterface))
                                : c.Get(null, null, typeof(TPolicyInterface))));
            }
        }
    }
}
