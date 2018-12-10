using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Selection;
using Unity.Builder.Strategy;
using Unity.Injection;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors;
using Unity.Policy;

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
        public override void PreBuildUp<TBuilderContext>(ref TBuilderContext context)
        {
            if (context.Existing != null)
            {
                return;
            }

            Type typeToBuild = context.BuildKey.Type;

            var interceptionPolicy = FindInterceptionPolicy<TBuilderContext, ITypeInterceptionPolicy>(ref context);
            if (interceptionPolicy == null)
            {
                return;
            }

            var interceptor = interceptionPolicy.GetInterceptor(ref context);
            if (!interceptor.CanIntercept(typeToBuild))
            {
                return;
            }

            var interceptionBehaviorsPolicy = FindInterceptionPolicy<TBuilderContext, IInterceptionBehaviorsPolicy>(ref context);

            IEnumerable<IInterceptionBehavior> interceptionBehaviors =
                interceptionBehaviorsPolicy == null
                    ?
                        Enumerable.Empty<IInterceptionBehavior>()
                    :
                        interceptionBehaviorsPolicy.GetEffectiveBehaviors(
                            ref context, interceptor, typeToBuild, typeToBuild)
                        .Where(ib => ib.WillExecute);

            IAdditionalInterfacesPolicy additionalInterfacesPolicy =
                FindInterceptionPolicy<TBuilderContext, IAdditionalInterfacesPolicy>(ref context);

            IEnumerable<Type> additionalInterfaces =
                additionalInterfacesPolicy != null ? additionalInterfacesPolicy.AdditionalInterfaces : Type.EmptyTypes;

            var enumerable = interceptionBehaviors as IInterceptionBehavior[] ?? interceptionBehaviors.ToArray();
            context.Registration.Set(typeof(EffectiveInterceptionBehaviorsPolicy), 
                new EffectiveInterceptionBehaviorsPolicy { Behaviors = enumerable });

            Type[] allAdditionalInterfaces =
                Intercept.GetAllAdditionalInterfaces(enumerable, additionalInterfaces);

            Type interceptingType =
                interceptor.CreateProxyType(typeToBuild, allAdditionalInterfaces);

            DerivedTypeConstructorSelectorPolicy.SetPolicyForInterceptingType(ref context, interceptingType);
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <remarks>In this class, PostBuildUp checks to see if the object was proxyable,
        /// and if it was, wires up the handlers.</remarks>
        /// <param name="context">Context of the build operation.</param>
        /// <param name="pre"></param>
        public override void PostBuildUp<TBuilderContext>(ref TBuilderContext context)
        {
            IInterceptingProxy proxy = context.Existing as IInterceptingProxy;

            if (proxy == null)
            {
                return;
            }

            var effectiveInterceptionBehaviorsPolicy =
                (EffectiveInterceptionBehaviorsPolicy)context.Policies
                                                             .Get(context.OriginalBuildKey.Type, 
                                                                  context.OriginalBuildKey.Name, 
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


        private static TPolicy FindInterceptionPolicy<TBuilderContext, TPolicy>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            return (TPolicy)(context.Policies.GetOrDefault(typeof(TPolicy), context.OriginalBuildKey) ??
                   (TPolicy)context.Policies.GetOrDefault(typeof(TPolicy), context.OriginalBuildKey.Type));
        }

        #endregion


        #region IRegisterTypeStrategy

        //public void RegisterType(IContainerContext context, Type typeFrom, Type typeTo, string name, 
        //                         LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        //{
        //    Type typeToBuild = typeFrom ?? typeTo;

        //    var policy = (ITypeInterceptionPolicy)(context.Policies.Get(typeToBuild, name, typeof(ITypeInterceptionPolicy), out _) ??
        //                                           context.Policies.Get(typeToBuild, string.Empty, typeof(ITypeInterceptionPolicy), out _));
        //    if (policy == null) return;

        //    var interceptor = policy.GetInterceptor(context.Container);
        //    if (typeof(VirtualMethodInterceptor) == interceptor?.GetType())
        //        context.Policies.Set(typeToBuild, name, typeof(IBuildPlanPolicy), new OverriddenBuildPlanMarkerPolicy());
        //}

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

        private class DerivedTypeConstructorSelectorPolicy : IConstructorSelectorPolicy
        {
            internal readonly Type _interceptingType;
            internal readonly IConstructorSelectorPolicy _originalConstructorSelectorPolicy;

            internal DerivedTypeConstructorSelectorPolicy(
                Type interceptingType,
                IConstructorSelectorPolicy originalConstructorSelectorPolicy)
            {
                _interceptingType = interceptingType;
                _originalConstructorSelectorPolicy = originalConstructorSelectorPolicy;
            }

            public object SelectConstructor<TContext>(ref TContext context)
                where TContext : IBuilderContext
            {
                object originalConstructor =
                    _originalConstructorSelectorPolicy.SelectConstructor(ref context);

                switch (originalConstructor)
                {
                    case ConstructorInfo info:
                        return FromConstructorInfo(info, _interceptingType);

                    case SelectedConstructor selectedConstructor:
                        return FromSelectedConstructor(selectedConstructor, _interceptingType);

                    case InjectionConstructor ctor:
                        return FromSelectedConstructor(
                            new SelectedConstructor(ctor.GetInfo(context.Type), ctor.GetParameters()), _interceptingType);
                }

                throw new InvalidOperationException("Unknown type");
            }

            private static SelectedConstructor FromSelectedConstructor(SelectedConstructor selectedConstructor, Type interceptingType)
            {
                var originalParams =selectedConstructor.Constructor.GetParameters();

                var newConstructorInfo =
                    interceptingType.GetConstructor(originalParams.Select(pi => pi.ParameterType).ToArray());

                var newConstructor = new SelectedConstructor(newConstructorInfo);

                foreach (var resolver in selectedConstructor.GetParameterResolvers())
                {
                    newConstructor.AddParameterResolver(resolver);
                }

                return newConstructor;
            }

            private static SelectedConstructor FromConstructorInfo(ConstructorInfo info, Type interceptingType)
            {
                var originalParams = info.GetParameters();

                var newConstructorInfo = interceptingType.GetConstructor(originalParams.Select(pi => pi.ParameterType).ToArray());

                return new SelectedConstructor(newConstructorInfo);
            }

            public static void SetPolicyForInterceptingType<TBuilderContext>(ref TBuilderContext context, Type interceptingType)
                where TBuilderContext : IBuilderContext
            {
                var currentSelectorPolicy =
                    (IConstructorSelectorPolicy)context.Policies.GetOrDefault(typeof(IConstructorSelectorPolicy),
                                                                              context.OriginalBuildKey);
                if (!(currentSelectorPolicy is DerivedTypeConstructorSelectorPolicy currentDerivedTypeSelectorPolicy))
                {
                    context.Registration.Set(typeof(IConstructorSelectorPolicy),
                                                  new DerivedTypeConstructorSelectorPolicy(
                                                      interceptingType, currentSelectorPolicy));
                }
                else if (currentDerivedTypeSelectorPolicy._interceptingType != interceptingType)
                {
                    context.Registration.Set(typeof(IConstructorSelectorPolicy),
                                                  new DerivedTypeConstructorSelectorPolicy(
                                                      interceptingType,
                                                      currentDerivedTypeSelectorPolicy._originalConstructorSelectorPolicy));
                }
            }
        }

        #endregion
    }
}
