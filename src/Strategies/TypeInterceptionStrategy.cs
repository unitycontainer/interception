using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Extension;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors;

namespace Unity.Interception.ContainerIntegration.ObjectBuilder
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that hooks up type interception. It looks for
    /// a <see cref="ITypeInterceptionPolicy"/> for the current build key, or the current
    /// build type. If present, it substitutes types so that proxy class gets
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
        /// class is proxy-able, and if so, replacing it with a proxy class.</remarks>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp<TContext>(ref TContext context)
        {
            if (context.Existing != null) return;

            var interceptor = context.OfType<Interceptor>()
                                     .FirstOrDefault(o => o.IsTypeInterceptor)?
                                     .GetInterceptor(ref context) as ITypeInterceptor ??
                              GetPolicyOrDefault<TContext, ITypeInterceptor>(ref context);

            if (interceptor is null) return;



            Type typeToBuild = context.Type;

            if (!interceptor.CanIntercept(typeToBuild))
            {
                return;
            }

            var behavior = (InterceptionBehavior?)context.Registration?.Get(typeof(InterceptionBehavior));

            var interceptionBehaviorsPolicy = GetPolicyOrDefault<TContext, IInterceptionBehaviorsPolicy>(ref context);

            IEnumerable<IInterceptionBehavior> interceptionBehaviors =
                interceptionBehaviorsPolicy == null
                    ?
                        Enumerable.Empty<IInterceptionBehavior>()
                    :
                        interceptionBehaviorsPolicy.GetEffectiveBehaviors(
                            ref context, interceptor, typeToBuild, typeToBuild)
                        .Where(ib => ib.WillExecute);


            IEnumerable<Type> additionalInterfaces = context.OfType<AdditionalInterface>()
                                                            .Select(a => a.InterfaceType);

            var enumerable = interceptionBehaviors as IInterceptionBehavior[] ?? interceptionBehaviors.ToArray();
            
            context.Set(typeof(EffectiveInterceptionBehaviorsPolicy), new EffectiveInterceptionBehaviorsPolicy { Behaviors = enumerable });

            Type[] allAdditionalInterfaces =
                Intercept.GetAllAdditionalInterfaces(enumerable, additionalInterfaces);

            Type interceptingType =
                interceptor.CreateProxyType(typeToBuild, allAdditionalInterfaces);

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

        public static TPolicy? GetPolicyOrDefault<TContext, TPolicy>(ref TContext context)
            where TContext : IBuilderContext
            where TPolicy : class
        {
            return context.Contract.Type.IsGenericType
                ? context.Policies.Get<TPolicy>(context.Contract.Type) ??
                  context.Policies.Get<TPolicy>(context.Generic ??= context.Contract.Type.GetGenericTypeDefinition())
                : context.Policies.Get<TPolicy>(context.Contract.Type);
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

        //private class DerivedTypeConstructorSelectorPolicy : ISelect<ConstructorInfo>
        //{
        //    internal readonly Type InterceptingType;
        //    internal readonly ISelect<ConstructorInfo> OriginalConstructorSelectorPolicy;

        //    internal DerivedTypeConstructorSelectorPolicy(
        //        Type interceptingType,
        //        ISelect<ConstructorInfo> originalConstructorSelectorPolicy)
        //    {
        //        InterceptingType = interceptingType;
        //        OriginalConstructorSelectorPolicy = originalConstructorSelectorPolicy;
        //    }


        //    public IEnumerable<object> Select(Type type, IPolicySet registration)
        //    {
        //        object originalConstructor =
        //            OriginalConstructorSelectorPolicy.Select(type, registration)
        //                                             .First();
        //        switch (originalConstructor)
        //        {
        //            case ConstructorInfo info:
        //                return new []{ FromConstructorInfo(info, InterceptingType) };

        //            case SelectedConstructor selectedConstructor:
        //                return new []{ FromSelectedConstructor(selectedConstructor, InterceptingType) };

        //            case InjectionMethodBase<ConstructorInfo> methodBaseMember:
        //                return new []{ FromMethodBaseMember(methodBaseMember, InterceptingType) };
        //        }

        //        throw new InvalidOperationException("Unknown type");
        //    }


        //    private static SelectedConstructor FromMethodBaseMember(InjectionMethodBase<ConstructorInfo> methodBaseMember, Type interceptingType)
        //    {
        //        return new SelectedConstructor(methodBaseMember.MemberInfo(interceptingType), methodBaseMember.Data);
        //    }

        //    private static SelectedConstructor FromSelectedConstructor(SelectedConstructor selectedConstructor, Type interceptingType)
        //    {
        //        var newConstructorInfo = interceptingType.GetConstructor(selectedConstructor.Info.GetParameters().Select(pi => pi.ParameterType).ToArray()); 
        //        var newConstructor = new SelectedConstructor(newConstructorInfo, selectedConstructor.Data);

        //        return newConstructor;
        //    }

        //    private static SelectedConstructor FromConstructorInfo(ConstructorInfo info, Type interceptingType)
        //    {
        //        var newConstructorInfo = interceptingType.GetConstructor(info.GetParameters().Select(pi => pi.ParameterType).ToArray());
        //        return new SelectedConstructor(newConstructorInfo);
        //    }

        //    public static void SetPolicyForInterceptingType<TContext>(ref TContext context, Type interceptingType)
        //        where TContext : IBuilderContext
        //    {
        //        //var currentSelectorPolicy =
        //        //    GetPolicy<ISelect<ConstructorInfo>>(ref context);

        //        //if (!(currentSelectorPolicy is DerivedTypeConstructorSelectorPolicy currentDerivedTypeSelectorPolicy))
        //        //{
        //        //    context.Registration.Set(typeof(ISelect<ConstructorInfo>),
        //        //        new DerivedTypeConstructorSelectorPolicy(interceptingType, currentSelectorPolicy));
        //        //}
        //        //else if (currentDerivedTypeSelectorPolicy.InterceptingType != interceptingType)
        //        //{
        //        //    context.Registration.Set(typeof(ISelect<ConstructorInfo>),
        //        //        new DerivedTypeConstructorSelectorPolicy(interceptingType, 
        //        //            currentDerivedTypeSelectorPolicy.OriginalConstructorSelectorPolicy));
        //        //}
        //    }
        //}

        #endregion
    }
}
