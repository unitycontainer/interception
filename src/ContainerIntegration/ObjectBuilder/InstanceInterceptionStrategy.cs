using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors;
using Unity.Policy;

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
        public override void PostBuildUp<TBuilderContext>(ref TBuilderContext context)
        {
            // If it's already been intercepted, don't do it again.
            if (context.Existing is IInterceptingProxy)
            {
                return;
            }

            IInstanceInterceptionPolicy interceptionPolicy =
                FindInterceptionPolicy<TBuilderContext, IInstanceInterceptionPolicy>(ref context, true);
            if (interceptionPolicy == null)
            {
                return;
            }

            var interceptor = interceptionPolicy.GetInterceptor(ref context);

            IInterceptionBehaviorsPolicy interceptionBehaviorsPolicy =
                FindInterceptionPolicy<TBuilderContext, IInterceptionBehaviorsPolicy>(ref context, true);
            if (interceptionBehaviorsPolicy == null)
            {
                return;
            }

            IAdditionalInterfacesPolicy additionalInterfacesPolicy =
                FindInterceptionPolicy<TBuilderContext, IAdditionalInterfacesPolicy>(ref context, false);
            IEnumerable<Type> additionalInterfaces =
                additionalInterfacesPolicy != null ? additionalInterfacesPolicy.AdditionalInterfaces : Type.EmptyTypes;

            Type typeToIntercept = context.OriginalBuildKey.Type;
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

        private static T FindInterceptionPolicy<TBuilderContext, T>(ref TBuilderContext context, bool probeOriginalKey)
            where TBuilderContext : IBuilderContext
            where T : class
        {
            // First, try for an original build key
            var policy = (T)context.Policies.GetOrDefault(typeof(T), context.OriginalBuildKey) ??
                         (T)context.Policies.GetOrDefault(typeof(T), context.OriginalBuildKey.Type);

            if (policy != null)
            {
                return policy;
            }

            if (!probeOriginalKey)
            {
                return null;
            }

            // Next, try the build type
            policy = (T)context.Policies.GetOrDefault(typeof(T), context.BuildKey) ??
                     (T)context.Policies.GetOrDefault(typeof(T), context.BuildKey.Type);

            return policy;
        }
    }
}
