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
    /// A <see cref="BuilderStrategy"/> that hooks up type interception. It looks for
    /// a <see cref="ITypeInterceptionPolicy"/> for the current build key, or the current
    /// build type. If present, it substitutes types so that that proxy class gets
    /// built up instead. On the way back, it hooks up the appropriate handlers.
    /// </summary>
    public class TypeInterceptionStrategy : BuilderStrategy
    {
        #region BuilderStrategy

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <remarks>In this class, PreBuildUp is responsible for figuring out if the
        /// class is proxyable, and if so, replacing it with a proxy class.</remarks>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp(ref BuilderContext context)
        {
            if (context.Existing != null)
            {
                return;
            }

            Type typeToBuild = context.Type;

            var interceptionPolicy = GetPolicyOrDefault<ITypeInterceptionPolicy>(ref context);
            if (interceptionPolicy == null)
            {
                return;
            }

            var interceptor = interceptionPolicy.GetInterceptor(ref context);
            if (!interceptor.CanIntercept(typeToBuild))
            {
                return;
            }

            var interceptionBehaviorsPolicy = GetPolicyOrDefault<IInterceptionBehaviorsPolicy>(ref context);

            IEnumerable<IInterceptionBehavior> interceptionBehaviors =
                interceptionBehaviorsPolicy == null
                    ?
                        Enumerable.Empty<IInterceptionBehavior>()
                    :
                        interceptionBehaviorsPolicy.GetEffectiveBehaviors(
                            ref context, interceptor, typeToBuild, typeToBuild)
                        .Where(ib => ib.WillExecute);

            IAdditionalInterfacesPolicy? additionalInterfacesPolicy =
                GetPolicyOrDefault<IAdditionalInterfacesPolicy>(ref context);

            IEnumerable<Type> additionalInterfaces =
                additionalInterfacesPolicy != null ? additionalInterfacesPolicy.AdditionalInterfaces : Type.EmptyTypes;

            var enumerable = interceptionBehaviors as IInterceptionBehavior[] ?? interceptionBehaviors.ToArray();
            context.Registration.Set(typeof(EffectiveInterceptionBehaviorsPolicy), 
                new EffectiveInterceptionBehaviorsPolicy { Behaviors = enumerable });

            Type[] allAdditionalInterfaces =
                Intercept.GetAllAdditionalInterfaces(enumerable, additionalInterfaces);

            context.Type = interceptor.CreateProxyType(typeToBuild, allAdditionalInterfaces);
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <remarks>In this class, PostBuildUp checks to see if the object was proxyable,
        /// and if it was, wires up the handlers.</remarks>
        /// <param name="context">Context of the build operation.</param>
        public override void PostBuildUp(ref BuilderContext context)
        {
            if (!(context.Existing is IInterceptingProxy proxy))
            {
                return;
            }

            var effectiveInterceptionBehaviorsPolicy = (EffectiveInterceptionBehaviorsPolicy?)context.Registration.Get(
                typeof(EffectiveInterceptionBehaviorsPolicy));

            if (effectiveInterceptionBehaviorsPolicy == null)
            {
                return;
            }

            foreach (var interceptionBehavior in effectiveInterceptionBehaviorsPolicy.Behaviors)
            {
                proxy.AddInterceptionBehavior(interceptionBehavior);
            }
        }

        public static TPolicyInterface? GetPolicyOrDefault<TPolicyInterface>(ref BuilderContext context) where TPolicyInterface : class
        {
            return (TPolicyInterface?)(GetNamedPolicy(ref context, context.RegistrationType, context.Name) ?? 
                                      GetNamedPolicy(ref context, context.RegistrationType, UnityContainer.All));

            static object? GetNamedPolicy(ref BuilderContext c, Type t, string? n)
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

        #endregion


        #region Nested Types


        private class EffectiveInterceptionBehaviorsPolicy 
        {
            public EffectiveInterceptionBehaviorsPolicy()
            {
                Behaviors = new List<IInterceptionBehavior>();
            }

            public IEnumerable<IInterceptionBehavior> Behaviors { get; set; }
        }

        #endregion
    }
}
