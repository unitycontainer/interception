using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Interception.Interceptors;
using Unity.Interception.PolicyInjection.MatchingRules;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;

namespace PolicyInjection
{
    /// <summary>
    /// Tests for the Policy class
    /// </summary>
    [TestClass]
    public class PolicyFixture
    {
        #region Fields

        private IUnityContainer Container;

        #endregion


        #region Scaffolding

        [TestInitialize]
        public void SetUp() => Container = new UnityContainer()
                .RegisterType<ICallHandler, Handler1>("handler1")
                .RegisterType<ICallHandler, Handler2>("handler2")
                .RegisterType<ICallHandler, Handler3>("handler3");

        #endregion


        [TestMethod("No handle, no match")]
        public void ShouldHaveNoHandlersWhenPolicyDoesntMatch()
        {
            IMatchingRule[] rules = { };

            InjectionPolicy p = CreatePolicy(Container, rules);

            MethodImplementationInfo thisMember = GetMethodImplInfo<PolicyFixture>("ShouldHaveNoHandlersWhenPolicyDoesntMatch");
            List<ICallHandler> memberHandlers
                = new List<ICallHandler>(p.GetHandlersFor(thisMember, Container));
            Assert.AreEqual(0, memberHandlers.Count);
        }

        [TestMethod("Same Order With GetHandlersFor")]
        public void ShouldGetHandlersInOrderWithGetHandlersFor()
        {
            IMatchingRule[] rules = { new MemberNameMatchingRule("ShouldGetHandlersInOrderWithGetHandlersFor") };

            InjectionPolicy p = CreatePolicy(Container, rules);

            MethodImplementationInfo member = GetMethodImplInfo<PolicyFixture>("ShouldGetHandlersInOrderWithGetHandlersFor");

            List<ICallHandler> expectedHandlers = new List<ICallHandler>(Container.ResolveAll<ICallHandler>());
            List<ICallHandler> actualHandlers = new List<ICallHandler>(p.GetHandlersFor(member, Container));

            CollectionAssertExtensions.AreEqual(
                expectedHandlers,
                actualHandlers,
                new TypeComparer());
        }

        [TestMethod("Match PropertyGet")]
        public void ShouldBeAbleToMatchPropertyGet()
        {
            IMatchingRule[] rules = { new MemberNameMatchingRule("get_Balance") };

            InjectionPolicy p = CreatePolicy(Container, rules);

            PropertyInfo balanceProperty = typeof(MockDal).GetProperty("Balance");
            MethodImplementationInfo getMethod = new MethodImplementationInfo(null, balanceProperty.GetGetMethod());

            List<ICallHandler> expectedHandlers = new List<ICallHandler>(Container.ResolveAll<ICallHandler>());
            List<ICallHandler> actualHandlers = new List<ICallHandler>(p.GetHandlersFor(getMethod, Container));

            CollectionAssertExtensions.AreEqual(
                expectedHandlers,
                actualHandlers,
                new TypeComparer());
        }

        private static InjectionPolicy CreatePolicy(IUnityContainer container, IMatchingRule[] rules) 
            => new RuleDrivenPolicy(rules, new string[] { "handler1", "handler2", "handler3" });

        private static MethodImplementationInfo GetMethodImplInfo<T>(string methodName) 
            => new MethodImplementationInfo(null, typeof(T).GetMethod(methodName));
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
