using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Injection;
using Unity.Interception;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;
using Unity.Interception.PolicyInjection;
using Unity.Interception.TestSupport;

namespace Configuration
{
    [TestClass]
    public class InterceptionConfigurationFixture
    {
        #region Fields

        const string METHOD = "Method";
        const string ADDPOLICY = nameof(Interception.AddPolicy);
        private IUnityContainer Container;

        #endregion


        #region Scaffolding

        [TestInitialize]
        public void SetUp() => Container = new UnityContainer()
            .AddNewExtension<Interception>();

        #endregion



        [TestMethod("Interceptor Through Injection"), TestProperty(METHOD, nameof(InjectionMember))]
        public void CanSetUpInterceptorThroughInjectionMember()
        {
            InvokeCountHandler handler = new InvokeCountHandler();

            Container.Configure<Interception>()
                .AddPolicy("policy")
                    .AddMatchingRule<AlwaysMatchingRule>()
                    .AddCallHandler(handler);

            Container.RegisterType<IInterface, BaseClass>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<PolicyInjectionBehavior>());

            IInterface instance = Container.Resolve<IInterface>();

            instance.DoSomething("1");

            Assert.AreEqual(1, handler.Count);
        }

        [TestMethod("Interceptor Through Injection (named)"), TestProperty(METHOD, nameof(InjectionMember))]
        public void CanSetUpInterceptorThroughInjectionMemberNamed()
        {
            InvokeCountHandler handler = new InvokeCountHandler();

            Container.Configure<Interception>()
                .AddPolicy("policy")
                    .AddMatchingRule<AlwaysMatchingRule>()
                    .AddCallHandler(handler);

            Container.RegisterType<IInterface, BaseClass>("test",
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<PolicyInjectionBehavior>());

            IInterface instance = Container.Resolve<IInterface>("test");

            instance.DoSomething("1");

            Assert.AreEqual(1, handler.Count);
        }

        [TestMethod]
        public void CanSetUpInterceptorThroughInjectionMemberForExistingInterceptor()
        {
            CallCountInterceptionBehavior interceptionBehavior = new CallCountInterceptionBehavior();

            Container.RegisterType<IInterface, BaseClass>(
                "test",
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior(interceptionBehavior));

            IInterface instance = Container.Resolve<IInterface>("test");

            instance.DoSomething("1");

            Assert.AreEqual(1, interceptionBehavior.CallCount);
        }

        [TestMethod]
        public void CanSetUpAdditionalInterfaceThroughInjectionMemberForInstanceInterception()
        {
            int invokeCount = 0;

            Container.RegisterType<IInterface, BaseClass>("test",
                                                          new Interceptor<InterfaceInterceptor>(),
                                                          new AdditionalInterface(typeof(IOtherInterface)),
                                                          new InterceptionBehavior(
                                                              new TestDelegateBehavior(
                                                                  (mi, gn) => { invokeCount++; return mi.CreateMethodReturn(0); })));

            IInterface instance = Container.Resolve<IInterface>("test");

            ((IOtherInterface)instance).DoSomethingElse("1");

            Assert.AreEqual(1, invokeCount);
        }

        [TestMethod]
        public void CanSetUpAdditionalInterfaceThroughInjectionMemberForTypeInterception()
        {
            int invokeCount = 0;

            Container.RegisterType<IInterface, BaseClass>(
                "test",
                new Interceptor<VirtualMethodInterceptor>(),
                new AdditionalInterface(typeof(IOtherInterface)),
                new InterceptionBehavior(
                    new TestDelegateBehavior(
                        (mi, gn) => { invokeCount++; return mi.CreateMethodReturn(0); })));

            IInterface instance = Container.Resolve<IInterface>("test");

            ((IOtherInterface)instance).DoSomethingElse("1");

            Assert.AreEqual(1, invokeCount);
        }

        [TestMethod]
        public void CanSetUpAdditionalInterfaceThroughGenericInjectionMemberForTypeInterception()
        {
            int invokeCount = 0;

            Container.RegisterType<IInterface, BaseClass>(
                "test",
                new Interceptor<VirtualMethodInterceptor>(),
                new AdditionalInterface<IOtherInterface>(),
                new InterceptionBehavior(
                    new TestDelegateBehavior(
                        (mi, gn) => { invokeCount++; return mi.CreateMethodReturn(0); })));

            IInterface instance = Container.Resolve<IInterface>("test");

            ((IOtherInterface)instance).DoSomethingElse("1");

            Assert.AreEqual(1, invokeCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConfiguringAnAdditionalInterfaceWithANonInterfaceTypeThrows()
        {
            new AdditionalInterface(typeof(int));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConfiguringAnAdditionalInterfaceWithANullTypeThrows()
        {
            new AdditionalInterface(null);
        }
    }

    #region Test Data


    public class BaseClass : IInterface
    {
        public int DoSomething(string param)
        {
            return int.Parse(param);
        }

        public int Property
        {
            get;
            set;
        }
    }

    public interface IInterface
    {
        int DoSomething(string param);
        int Property { get; set; }
    }

    public interface IOtherInterface
    {
        int DoSomethingElse(string param);
    }
    
    #endregion
}
