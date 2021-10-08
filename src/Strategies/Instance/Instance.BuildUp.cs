using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Extension;

namespace Unity.Interception.Strategies
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that intercepts objects
    /// in the build chain by creating a proxy object.
    /// </summary>
    public partial class InstanceInterceptionStrategy
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
            if (context.Existing is IInterceptingProxy) return;

            try
            {
                var interceptor = GetInterceptor(ref context);
                if (interceptor is null) return;

                var injection = context.FirstOrDefault<InterceptionBehavior>();
                var behaviors = injection is null
                    ? GetInjectedBehaviors(injection, context.Overrides)
                    : GetPolicyBehaviors(interceptor);


                var interceptionBehaviors = InterceptionBehavior.GetEffectiveBehaviors(ref context, interceptor);

                IEnumerable<Type> additionalInterfaces = context.OfType<AdditionalInterface>()
                                                                .Select(a => a.InterfaceType);

                if (interceptionBehaviors.Length > 0)
                {
                    context.Existing =
                        Intercept.ThroughProxyWithAdditionalInterfaces(
                            context.Contract.Type,
                            context.Existing,
                            interceptor,
                            interceptionBehaviors,
                            additionalInterfaces);
                }
            }
            catch (Exception ex)
            {
                context.Error(ex.Message);
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

        public static TPolicy? GetPolicyOrDefault<TContext, TPolicy>(ref TContext context)
            where TContext : IBuilderContext
            where TPolicy : class
        {
            return context.Contract.Type.IsGenericType
                ? context.Policies.Get<TPolicy>(context.Contract.Type) ??
                  context.Policies.Get<TPolicy>(context.TypeDefinition!)
                : context.Policies.Get<TPolicy>(context.Contract.Type);
        }
    }
}
