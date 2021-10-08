using System.Collections.Generic;
using Unity.Extension;
using Unity.Interception.ContainerIntegration;

namespace Unity.Interception.Strategies
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that hooks up type interception. It looks for
    /// a <see cref="ITypeInterceptionPolicy"/> for the current build key, or the current
    /// build type. If present, it substitutes types so that proxy class gets
    /// built up instead. On the way back, it hooks up the appropriate handlers.
    /// </summary>
    public partial class TypeInterceptionStrategy : InterceptionStrategy
    {
        #region Constructors

        public TypeInterceptionStrategy(Interception extension)
            : base(extension)
        { }

        #endregion


        #region Nested Types


        private class EffectiveInterceptionBehaviorsPolicy : InterceptionMember
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
