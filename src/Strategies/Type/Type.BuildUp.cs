using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Extension;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.ContainerIntegration.ObjectBuilder;

namespace Unity.Interception.Strategies
{
    public partial class TypeInterceptionStrategy
    {
        #region BuilderStrategy

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <remarks>In this class, PreBuildUp is responsible for figuring out if the
        /// class is proxy-able, and if so, replacing it with a proxy class.</remarks>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp<TContext>(ref TContext context)
        {
            if (context.Existing != null) return;

            ITypeInterceptor? interceptor = 
                context.OfType<Interceptor>(o => o.IsTypeInterceptor)?
                       .GetInterceptor(ref context) as ITypeInterceptor ??
                PolicyOrDefault<TContext, ITypeInterceptor>(ref context);
            
            if (interceptor is null) return;


            Type typeToBuild = context.Type;


            var behavior = (InterceptionBehavior?)context.Registration?.Get(typeof(InterceptionBehavior));

            var interceptionBehaviorsPolicy = PolicyOrDefault<TContext, IInterceptionBehaviorsPolicy>(ref context);

            IEnumerable<IInterceptionBehavior> interceptionBehaviors =
                interceptionBehaviorsPolicy == null
                    ?
                        Enumerable.Empty<IInterceptionBehavior>()
                    : Enumerable.Empty<IInterceptionBehavior>();
                        //interceptionBehaviorsPolicy.GetEffectiveBehaviors(
                        //    ref context, interceptor, typeToBuild, typeToBuild)
                        //.Where(ib => ib.WillExecute);


            IEnumerable<Type> additionalInterfaces = context.OfType<AdditionalInterface>()
                                                            .Select(a => a.InterfaceType);

            var enumerable = interceptionBehaviors as IInterceptionBehavior[] ?? interceptionBehaviors.ToArray();
            
            context.Set(typeof(EffectiveInterceptionBehaviorsPolicy), new EffectiveInterceptionBehaviorsPolicy { Behaviors = enumerable });

            Type[] allAdditionalInterfaces =
                Intercept.GetAllAdditionalInterfaces(enumerable, additionalInterfaces);

            //Type interceptingType =
            //    ((ITypeInterceptor)interceptor.GetInterceptor(ref context))
            //                                  .CreateProxyType(typeToBuild, allAdditionalInterfaces);

            //DerivedTypeConstructorSelectorPolicy.SetPolicyForInterceptingType(ref context, interceptingType);
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <remarks>In this class, PostBuildUp checks to see if the object was proxyable,
        /// and if it was, wires up the handlers.</remarks>
        /// <param name="context">Context of the build operation.</param>
        public override void PostBuildUp<TContext>(ref TContext context)
        {
            IInterceptingProxy proxy = context.Existing as IInterceptingProxy;

            if (proxy == null)
            {
                return;
            }

            var effectiveInterceptionBehaviorsPolicy = (EffectiveInterceptionBehaviorsPolicy)context.Registration.Get(
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

        public static TPolicy? PolicyOrDefault<TContext, TPolicy>(ref TContext context)
            where TContext : IBuilderContext
            where TPolicy : class
        {
            return context.Contract.Type.IsGenericType
                 ? context.Policies.Get<TPolicy>(context.Contract.Type) ??
                   context.Policies.Get<TPolicy>(context.Generic ??= context.Contract.Type.GetGenericTypeDefinition())
                 : context.Policies.Get<TPolicy>(context.Contract.Type);
        }

        #endregion
    }
}
