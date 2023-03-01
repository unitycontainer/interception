﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Interception;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Interception.Interceptors.InstanceInterceptors.TransparentProxyInterception;
using Unity.Interception.PolicyInjection;
using Unity.Interception.PolicyInjection.MatchingRules;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.PolicyInjection
{
    public class ContainerConfiguredForPolicyInjectionContext
    {
        [TestInitialize]
        public void Setup()
        {
            Arrange();
        }

        protected virtual void Arrange()
        {
            Container =
                new UnityContainer()
                    .RegisterType<BaseClass, DerivedClass>("derived",
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<PolicyInjectionBehavior>())
                    .RegisterType<BaseClass, DerivedWithNoOverrideClass>("derived-nooverride",
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<PolicyInjectionBehavior>())
                    .RegisterType<BaseClass>(
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<PolicyInjectionBehavior>())
                    .AddNewExtension<Interception>()
                    .Configure<Interception>()
                        .AddPolicy("base")
                            .AddMatchingRule(new TypeMatchingRule(typeof(BaseClass)))
                            .AddMatchingRule(new MemberNameMatchingRule("InterceptedMethod"))
                            .AddCallHandler(new AppendSuffixCallHandler { Suffix = "-basehandler", Order = 1 })
                        .Interception
                        .AddPolicy("derived-no-override")
                            .AddMatchingRule(new TypeMatchingRule(typeof(DerivedWithNoOverrideClass)))
                            .AddMatchingRule(new MemberNameMatchingRule("InterceptedMethod"))
                            .AddCallHandler(new AppendSuffixCallHandler { Suffix = "-derivednooverridehandler", Order = 2 })
                        .Interception
                        .AddPolicy("derived")
                            .AddMatchingRule(new TypeMatchingRule(typeof(DerivedClass)))
                            .AddMatchingRule(new MemberNameMatchingRule("InterceptedMethod"))
                            .AddCallHandler(new AppendSuffixCallHandler { Suffix = "-derivedhandler", Order = 3 })
                        .Interception
                    .Container;
        }

        public IUnityContainer Container { get; set; }
    }

    [TestClass]
    public class GivenInterceptedInstanceOfDerivedTypeWithNoOverride : ContainerConfiguredForPolicyInjectionContext
    {
        public BaseClass Instance { get; set; }

        protected override void Arrange()
        {
            base.Arrange();

            Instance = Container.Resolve<BaseClass>("derived-nooverride");
        }
    }

    [TestClass]
    public class GivenInterceptedInstanceOfDerivedTypeWithOverride : ContainerConfiguredForPolicyInjectionContext
    {
        public BaseClass Instance { get; set; }

        protected override void Arrange()
        {
            base.Arrange();

            Instance = Container.Resolve<BaseClass>("derived");
        }
    }

    public class BaseClass
    {
        public virtual string InterceptedMethod()
        {
            return "base";
        }

        [AppendSuffixCallHandlerNoInheritance(Suffix = "-basehandler", Order = 1)]
        public virtual string AttributeNoInheritanceInterceptedMethod()
        {
            return "base";
        }

        [AppendSuffixCallHandlerInheritance(Suffix = "-basehandler", Order = 1)]
        public virtual string AttributeInheritanceInterceptedMethod()
        {
            return "base";
        }
    }

    public class DerivedClass : BaseClass
    {
        public override string InterceptedMethod()
        {
            return "derived";
        }

        [AppendSuffixCallHandlerNoInheritance(Suffix = "-derivedhandler", Order = 2)]
        public new virtual string AttributeNoInheritanceInterceptedMethod()
        {
            return "base";
        }

        [AppendSuffixCallHandlerInheritance(Suffix = "-derivedhandler", Order = 2)]
        public new virtual string AttributeInheritanceInterceptedMethod()
        {
            return "base";
        }
    }

    public class DerivedWithNoOverrideClass : BaseClass
    {
    }

    public class AppendSuffixCallHandler : ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var methodReturn = getNext()(input, getNext);
            if (methodReturn.ReturnValue != null && methodReturn.ReturnValue is string)
            {
                methodReturn.ReturnValue = ((string)methodReturn.ReturnValue) + this.Suffix;
            }

            return methodReturn;
        }

        public int Order { get; set; }

        public string Suffix { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class AppendSuffixCallHandlerNoInheritanceAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new AppendSuffixCallHandler { Suffix = this.Suffix, Order = this.Order };
        }

        public string Suffix { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class AppendSuffixCallHandlerInheritanceAttribute : AppendSuffixCallHandlerNoInheritanceAttribute
    {
    }
}
