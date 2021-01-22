using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Injection;
using Unity.Interception;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;
using Unity.Interception.PolicyInjection.MatchingRules;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Lifetime;

namespace Configuration
{
    public partial class PolicyFixture
    {
        [TestMethod("Policy With Rules And Handlers")]
        public void CanSetUpAPolicyWithGivenRulesAndHandlers()
        {
            IMatchingRule rule1 = new AlwaysMatchingRule();
            ICallHandler handler1 = new CallCountHandler();

            Container.Configure<Interception>()
                     .AddPolicy(PolicyName)
                        .AddMatchingRule(rule1)
                        .AddCallHandler(handler1);

            Container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(Name, new VirtualMethodInterceptor());

            Wrappable wrappable1 = Container.Resolve<Wrappable>(Name);
            wrappable1.Method2();

            Assert.AreEqual(1, ((CallCountHandler)handler1).CallCount);
        }

        [TestMethod]
        public void CanSetUpAPolicyWithGivenRulesAndHandlersTypes()
        {
            Container
                .Configure<Interception>()
                    .AddPolicy(PolicyName)
                        .AddMatchingRule(typeof(AlwaysMatchingRule))
                        .AddCallHandler(typeof(GlobalCountCallHandler));

            GlobalCountCallHandler.Calls.Clear();

            Container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(Name, new VirtualMethodInterceptor());

            Wrappable wrappable1 = Container.Resolve<Wrappable>(Name);
            wrappable1.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["default"]);
        }

        [TestMethod]
        public void CanSetUpAPolicyWithGivenRulesAndHandlersTypesWithGenerics()
        {
            Container
                .Configure<Interception>()
                    .AddPolicy(PolicyName)
                        .AddMatchingRule<AlwaysMatchingRule>()
                        .AddCallHandler<GlobalCountCallHandler>();

            GlobalCountCallHandler.Calls.Clear();

            Container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(Name, new VirtualMethodInterceptor());

            Wrappable wrappable1 = Container.Resolve<Wrappable>(Name);
            wrappable1.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["default"]);
        }

        [TestMethod]
        public void CanSetUpAPolicyWithInjectedRulesAndHandlers()
        {
            Container
                .Configure<Interception>()
                    .AddPolicy(PolicyName)
                        .AddMatchingRule<AlwaysMatchingRule>()
                        .AddCallHandler<GlobalCountCallHandler>(
                            new InjectionConstructor("handler1"))
                        .AddCallHandler<GlobalCountCallHandler>(
                            new InjectionConstructor("handler2"),
                            new InjectionProperty("Order", 10));

            GlobalCountCallHandler.Calls.Clear();

            Container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(Name, new VirtualMethodInterceptor());

            Wrappable wrappable1 = Container.Resolve<Wrappable>(Name);
            wrappable1.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler1"]);
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler2"]);
        }

        [TestMethod]
        public void CanSetUpAPolicyWithNonGenericInjectedRulesAndHandlers()
        {
            Container
                .Configure<Interception>()
                    .AddPolicy(PolicyName)
                        .AddMatchingRule(typeof(AlwaysMatchingRule))
                        .AddCallHandler(
                            typeof(GlobalCountCallHandler),
                            new InjectionConstructor("handler1"))
                        .AddCallHandler(
                            typeof(GlobalCountCallHandler),
                            new InjectionConstructor("handler2"),
                            new InjectionProperty("Order", 10));

            GlobalCountCallHandler.Calls.Clear();

            Container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(Name, new VirtualMethodInterceptor());

            Wrappable wrappable1 = Container.Resolve<Wrappable>(Name);
            wrappable1.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler1"]);
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler2"]);
        }

        [TestMethod]
        public void CanSetUpAPolicyWithExternallyConfiguredRulesAndHandlers()
        {
            Container
                .Configure<Interception>()
                    .AddPolicy(PolicyName)
                        .AddMatchingRule("rule1")
                        .AddCallHandler("handler1")
                        .AddCallHandler("handler2").Interception.Container
                .RegisterType<IMatchingRule, AlwaysMatchingRule>("rule1")
                .RegisterType<ICallHandler, GlobalCountCallHandler>(
                    "handler1",
                    new InjectionConstructor("handler1"))
                .RegisterType<ICallHandler, GlobalCountCallHandler>(
                    "handler2",
                    new InjectionConstructor("handler2"),
                    new InjectionProperty("Order", 10));

            GlobalCountCallHandler.Calls.Clear();

            Container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(Name, new VirtualMethodInterceptor());

            Wrappable wrappable1 = Container.Resolve<Wrappable>(Name);
            wrappable1.Method2();

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler1"]);
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler2"]);
        }

        [TestMethod]
        public void CanSetUpAPolicyWithNamedInjectedRulesAndHandlers()
        {
            Container
                .Configure<Interception>()
                    .AddPolicy(PolicyName)
                        .AddMatchingRule<AlwaysMatchingRule>("rule1")
                        .AddCallHandler<GlobalCountCallHandler>(
                            "handler1",
                            new InjectionConstructor("handler1"))
                        .AddCallHandler<GlobalCountCallHandler>(
                            "handler2",
                            new InjectionConstructor("handler2"),
                            new InjectionProperty("Order", 10));

            GlobalCountCallHandler.Calls.Clear();

            Container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(Name, new VirtualMethodInterceptor());

            Wrappable wrappable1 = Container.Resolve<Wrappable>(Name);
            wrappable1.Method2();

            GlobalCountCallHandler handler1 = (GlobalCountCallHandler)Container.Resolve<ICallHandler>("handler1");
            GlobalCountCallHandler handler2 = (GlobalCountCallHandler)Container.Resolve<ICallHandler>("handler2");

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler1"]);
            Assert.AreEqual(1, GlobalCountCallHandler.Calls["handler2"]);
            Assert.AreEqual(0, handler1.Order);
            Assert.AreEqual(10, handler2.Order);
        }

        [TestMethod]
        public void CanSetUpAPolicyWithLifetimeManagedNamedInjectedRulesAndHandlers()
        {
            Container
                .Configure<Interception>()
                    .AddPolicy(PolicyName)
                        .AddMatchingRule<AlwaysMatchingRule>(
                            "rule1",
                            new ContainerControlledLifetimeManager())
                        .AddCallHandler<CallCountHandler>(
                            "handler1",
                            (LifetimeManager)null)
                        .AddCallHandler<CallCountHandler>(
                            "handler2",
                            new ContainerControlledLifetimeManager(),
                            new InjectionProperty("Order", 10));

            GlobalCountCallHandler.Calls.Clear();

            Container
                .Configure<Interception>()
                    .SetInterceptorFor<Wrappable>(Name, new VirtualMethodInterceptor());

            Wrappable wrappable1 = Container.Resolve<Wrappable>(Name);
            wrappable1.Method2();

            CallCountHandler handler1 = (CallCountHandler)Container.Resolve<ICallHandler>("handler1");
            CallCountHandler handler2 = (CallCountHandler)Container.Resolve<ICallHandler>("handler2");

            Assert.AreEqual(0, handler1.CallCount);     // not lifetime maanaged
            Assert.AreEqual(1, handler2.CallCount);     // lifetime managed
        }

        [TestMethod]
        public void SettingUpAPolicyWithANullRuleElementThrows()
        {
            try
            {
                Container
                    .Configure<Interception>()
                        .AddPolicy(PolicyName)
                            .AddMatchingRule(typeof(AlwaysMatchingRule))
                            .AddMatchingRule((string)null)
                            .AddCallHandler(new CallCountHandler());
                Assert.Fail("Should have thrown");
            }
            catch (ArgumentException)
            {
            }
        }
    }
}
