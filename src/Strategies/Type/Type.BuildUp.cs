using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Extension;
using Unity.Interception.ContainerIntegration;

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

            var info = new RegistrationInfo(ref context);

            try
            {
                var interceptor = GetInterceptor(ref context);
                if (interceptor is null) return;

                var injection = context.FirstOrDefault<InterceptionBehavior>();
                var behaviors = injection is null
                    ? GetInjectedBehaviors(injection, context.Overrides) 
                    : GetPolicyBehaviors(interceptor);
            }
            catch (Exception ex)
            {
                context.Error(ex.Message);
                return;
            }


            Type typeToBuild = context.Type;
//            var interceptionBehaviorsPolicy = PolicyOrDefault<TContext, IInterceptionBehaviorsPolicy>(ref context);

            //IEnumerable<IInterceptionBehavior> interceptionBehaviors =
            //    interceptionBehaviorsPolicy == null
            //        ? Enumerable.Empty<IInterceptionBehavior>()
            //        : interceptionBehaviorsPolicy.GetEffectiveBehaviors(ref context, interceptor, context.Contract.Type, context.Type)
            //                                     .Where(ib => ib.WillExecute);

            //var injectedBehaviors = InterceptionBehavior.GetEffectiveBehaviors(ref context, interceptor);
            //var enumerable = interceptionBehaviors.Concat(injectedBehaviors).ToArray();

            //context.Set(typeof(EffectiveInterceptionBehaviorsPolicy), new EffectiveInterceptionBehaviorsPolicy { Behaviors = enumerable });

            //Type[] allAdditionalInterfaces = Intercept.GetAllAdditionalInterfaces(enumerable, context.OfType<AdditionalInterface>()
            //                                                                                         .Select(a => a.InterfaceType));

            //Type interceptingType = interceptor.CreateProxyType(typeToBuild, allAdditionalInterfaces);

            //context.AsType(interceptingType);
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

            var effectiveInterceptionBehaviorsPolicy = (EffectiveInterceptionBehaviorsPolicy)context.Get(
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

        #endregion
    }
}
