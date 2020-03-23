using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Interception.Interceptors;
using Unity.Interception.PolicyInjection.MatchingRules;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.Tests;

namespace Extension.Tests
{
    /// <summary>
    /// Tests for the Policy class
    /// </summary>
    [TestClass]
    public class PolicyFixture : TestFixtureBase
    {
        [TestMethod]
        [Ignore]
        public void ShouldInitializeToEmpty()
        {
            //RuleDrivenPolicy p = new RuleDrivenPolicy("Empty");
            //Assert.AreEqual("Empty", p.Name);
            //Assert.AreEqual(0, p.RuleSet.Count);
            //Assert.AreEqual(0, p.Handlers.Count);
        }

        [TestMethod]
        [Ignore]
        public void ShouldPreserveHandlerOrder()
        {
            //RuleDrivenPolicy p = new RuleDrivenPolicy("OrderedHandlers");

            //ICallHandler h1 = new Handler1();
            //ICallHandler h2 = new Handler2();
            //ICallHandler h3 = new Handler3();

            //p.Handlers.Add(h2);
            //p.Handlers.Add(h1);
            //p.Handlers.Add(h3);

            //Assert.AreEqual(3, p.Handlers.Count);
            //Assert.AreSame(h2, p.Handlers[0]);
            //Assert.AreSame(h1, p.Handlers[1]);
            //Assert.AreSame(h3, p.Handlers[2]);
        }

        [TestMethod]
        public void ShouldHaveNoHandlersWhenPolicyDoesntMatch()
        {
            IMatchingRule[] rules = { };
            InjectionPolicy p = CreatePolicy(rules);

            MethodImplementationInfo thisMember = GetMethodImplInfo<PolicyFixture>("ShouldHaveNoHandlersWhenPolicyDoesntMatch");
            List<ICallHandler> memberHandlers
                = new List<ICallHandler>(p.GetHandlersFor(thisMember, Container));
            Assert.AreEqual(0, memberHandlers.Count);
        }

        [TestMethod]
        public void ShouldGetHandlersInOrderWithGetHandlersFor()
        {
            IMatchingRule[] rules = { new MemberNameMatchingRule("ShouldGetHandlersInOrderWithGetHandlersFor") };

            InjectionPolicy p = CreatePolicy(rules);

            MethodImplementationInfo member = GetMethodImplInfo<PolicyFixture>("ShouldGetHandlersInOrderWithGetHandlersFor");

            List<ICallHandler> expectedHandlers = new List<ICallHandler>(Container.ResolveAll<ICallHandler>());
            List<ICallHandler> actualHandlers = new List<ICallHandler>(p.GetHandlersFor(member, Container));

            CollectionAssertExtensions.AreEqual(
                expectedHandlers,
                actualHandlers,
                new TypeComparer());
        }

        [TestMethod]
        public void ShouldBeAbleToMatchPropertyGet()
        {
            IMatchingRule[] rules = { new MemberNameMatchingRule("get_Balance") };

            InjectionPolicy p = CreatePolicy(rules);

            PropertyInfo balanceProperty = typeof(MockDal).GetProperty("Balance");
            MethodImplementationInfo getMethod = new MethodImplementationInfo(null, balanceProperty.GetGetMethod());

            List<ICallHandler> expectedHandlers = new List<ICallHandler>(Container.ResolveAll<ICallHandler>());
            List<ICallHandler> actualHandlers = new List<ICallHandler>(p.GetHandlersFor(getMethod, Container));

            CollectionAssertExtensions.AreEqual(
                expectedHandlers,
                actualHandlers,
                new TypeComparer());
        }

        private static InjectionPolicy CreatePolicy(IMatchingRule[] rules)
        {
            InjectionPolicy p
                = new RuleDrivenPolicy(rules, new string[] { "handler1", "handler2", "handler3" });
            return p;
        }

        public override IUnityContainer GetContainer()
        {
            IUnityContainer container = base.GetContainer()
                .RegisterType<ICallHandler, Handler1>("handler1")
                .RegisterType<ICallHandler, Handler2>("handler2")
                .RegisterType<ICallHandler, Handler3>("handler3");
            return container;
        }

        private static MethodImplementationInfo GetMethodImplInfo<T>(string methodName)
        {
            return new MethodImplementationInfo(null,
                typeof(T).GetMethod(methodName));
        }
    }


    #region Test Data

    public class Handler1 : ICallHandler
    {
        private int order = 0;

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public IMethodReturn Invoke(IMethodInvocation input,
                                    GetNextHandlerDelegate getNext)
        {
            throw new NotImplementedException();
        }
    }

    public class Handler2 : ICallHandler
    {
        private int order = 0;

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public IMethodReturn Invoke(IMethodInvocation input,
                                    GetNextHandlerDelegate getNext)
        {
            throw new NotImplementedException();
        }
    }

    public class Handler3 : ICallHandler
    {
        private int order = 0;

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public IMethodReturn Invoke(IMethodInvocation input,
                                    GetNextHandlerDelegate getNext)
        {
            throw new NotImplementedException();
        }
    }

    public interface IYetAnotherInterface
    {
        void MyMethod();
    }

    public class YetAnotherMyType : IYetAnotherInterface
    {
        public void MyMethod() { }
    }

    public class TypeComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x.GetType() == y.GetType())
            {
                return 0;
            }
            return -1;
        }
    }

    #endregion
}
