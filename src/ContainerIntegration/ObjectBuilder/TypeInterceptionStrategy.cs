using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors;
using Unity.Policy;
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

            var interceptionPolicy = FindInterceptionPolicy<ITypeInterceptionPolicy>(ref context);
            if (interceptionPolicy == null)
            {
                return;
            }

            var interceptor = interceptionPolicy.GetInterceptor(ref context);
            if (!interceptor.CanIntercept(typeToBuild))
            {
                return;
            }

            var interceptionBehaviorsPolicy = FindInterceptionPolicy<IInterceptionBehaviorsPolicy>(ref context);

            IEnumerable<IInterceptionBehavior> interceptionBehaviors =
                interceptionBehaviorsPolicy == null
                    ?
                        Enumerable.Empty<IInterceptionBehavior>()
                    :
                        interceptionBehaviorsPolicy.GetEffectiveBehaviors(
                            ref context, interceptor, typeToBuild, typeToBuild)
                        .Where(ib => ib.WillExecute);

            IAdditionalInterfacesPolicy additionalInterfacesPolicy =
                FindInterceptionPolicy<IAdditionalInterfacesPolicy>(ref context);

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
        public override void PostBuildUp(ref BuilderContext context)
        {
            IInterceptingProxy proxy = context.Existing as IInterceptingProxy;

            if (proxy == null)
            {
                return;
            }

            var effectiveInterceptionBehaviorsPolicy = (EffectiveInterceptionBehaviorsPolicy)context.Get(
                context.RegistrationType, context.RegistrationName, typeof(EffectiveInterceptionBehaviorsPolicy));

            if (effectiveInterceptionBehaviorsPolicy == null)
            {
                return;
            }

            foreach (var interceptionBehavior in effectiveInterceptionBehaviorsPolicy.Behaviors)
            {
                proxy.AddInterceptionBehavior(interceptionBehavior);
            }
        }


        private static TPolicy FindInterceptionPolicy<TPolicy>(ref BuilderContext context)
        {
            return GetPolicyOrDefault<TPolicy>(ref context, context.RegistrationType, context.RegistrationName);

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

        private class DerivedTypeConstructorSelectorPolicy : ISelect<ConstructorInfo>
        {
            internal readonly Type InterceptingType;
            internal readonly ISelect<ConstructorInfo> OriginalConstructorSelectorPolicy;

            internal DerivedTypeConstructorSelectorPolicy(
                Type interceptingType,
                ISelect<ConstructorInfo> originalConstructorSelectorPolicy)
            {
                InterceptingType = interceptingType;
                OriginalConstructorSelectorPolicy = originalConstructorSelectorPolicy;
            }

            public IEnumerable<object> Select(ref BuilderContext context)
            {
                object originalConstructor =
                    OriginalConstructorSelectorPolicy.Select(ref context)
                                                     .First();

                switch (originalConstructor)
                {
                    case ConstructorInfo info:
                        return new []{ FromConstructorInfo(info, InterceptingType) };

                    case SelectedConstructor selectedConstructor:
                        return new []{ FromSelectedConstructor(selectedConstructor, InterceptingType) };

                    case MethodBaseMember<ConstructorInfo> methodBaseMember:
                        return new []{ FromMethodBaseMember(methodBaseMember, InterceptingType) };
                }

                throw new InvalidOperationException("Unknown type");
            }


            private static SelectedConstructor FromMethodBaseMember(MethodBaseMember<ConstructorInfo> methodBaseMember, Type type)
            {
                var (cInfo, args) = methodBaseMember.FromType(type);

                var newConstructorInfo = type.GetConstructor(cInfo.GetParameters().Select(pi => pi.ParameterType).ToArray());

                return new SelectedConstructor(newConstructorInfo, args);
            }

            private static SelectedConstructor FromSelectedConstructor(SelectedConstructor selectedConstructor, Type interceptingType)
            {
                var originalParams = selectedConstructor.Constructor.GetParameters();

                var newConstructorInfo =
                    interceptingType.GetConstructor(originalParams.Select(pi => pi.ParameterType).ToArray());

                var newConstructor = new SelectedConstructor(newConstructorInfo, originalParams);

                return newConstructor;
            }

            private static SelectedConstructor FromConstructorInfo(ConstructorInfo info, Type interceptingType)
            {
                var originalParams = info.GetParameters();

                var newConstructorInfo = interceptingType.GetConstructor(originalParams.Select(pi => pi.ParameterType).ToArray());

                return new SelectedConstructor(newConstructorInfo);
            }

            public static void SetPolicyForInterceptingType(ref BuilderContext context, Type interceptingType)
            {
                var currentSelectorPolicy =
                    GetPolicy<ISelect<ConstructorInfo>>(ref context, context.RegistrationType, context.RegistrationName);

                if (!(currentSelectorPolicy is DerivedTypeConstructorSelectorPolicy currentDerivedTypeSelectorPolicy))
                {
                    context.Registration.Set(typeof(ISelect<ConstructorInfo>),
                        new DerivedTypeConstructorSelectorPolicy(interceptingType, currentSelectorPolicy));
                }
                else if (currentDerivedTypeSelectorPolicy.InterceptingType != interceptingType)
                {
                    context.Registration.Set(typeof(ISelect<ConstructorInfo>),
                        new DerivedTypeConstructorSelectorPolicy(interceptingType, 
                            currentDerivedTypeSelectorPolicy.OriginalConstructorSelectorPolicy));
                }
            }
        }

        #endregion
    }
}
