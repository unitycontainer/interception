using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Interception;
using Unity.Interception.Interceptors;
using Unity.Interception.PolicyInjection;
using Unity.Interception.PolicyInjection.MatchingRules;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;

namespace Unit.Tests
{
    /// <summary>
    /// Tests for the var class.
    /// </summary>
    [TestClass]
    public class CalculateHandlers
    {
        #region Fields

        private IUnityContainer Container;

        #endregion


        #region Scaffolding

        [TestInitialize]
        public void TestInitialize() => Container = new UnityContainer();

        #endregion


        [TestMethod("Add policy")]
        public void ShouldBeAbleToAddOnePolicy()
        {
            Container
                .RegisterInstance<ICallHandler>("Handler1", new Handler1())
                .RegisterInstance<ICallHandler>("Handler2", new Handler2());

            var policies = new List<InjectionPolicy>();

            RuleDrivenPolicy p
                = new RuleDrivenPolicy("NameMatching",
                    new IMatchingRule[] { new MemberNameMatchingRule("ShouldBeAbleToAddOnePolicy") },
                    new ICallHandler[] { new Handler1(), new Handler2() });

            policies.Add(p);

            MethodImplementationInfo thisMember = GetMethodImplInfo<CalculateHandlers>("ShouldBeAbleToAddOnePolicy");
            var handlers = PolicyInjectionBehavior.CalculateHandlersFor(policies, thisMember, Container).ToList();

            Assert.AreEqual(2, handlers.Count);
            Assert.IsTrue(typeof(Handler1) == handlers[0].GetType());
            Assert.IsTrue(typeof(Handler2) == handlers[1].GetType());
        }

        [TestMethod]
        public void ShouldMatchPolicyByTypeName()
        {
            var policies = GetMultiplePolicySet();

            MethodImplementationInfo nameDoesntMatchMember = GetMethodImplInfo<MatchesByType>("NameDoesntMatch");
            MethodImplementationInfo nameMatchMember = GetMethodImplInfo<MatchesByType>("NameMatch");

            List<ICallHandler> nameDoesntMatchHandlers =
                new List<ICallHandler>(PolicyInjectionBehavior.CalculateHandlersFor(policies, nameDoesntMatchMember, Container));
            List<ICallHandler> nameMatchHandlers =
                new List<ICallHandler>(PolicyInjectionBehavior.CalculateHandlersFor(policies, nameMatchMember, Container));

            Assert.AreEqual(1, nameDoesntMatchHandlers.Count);
            Assert.IsTrue(typeof(Handler1) == nameDoesntMatchHandlers[0].GetType());

            Assert.AreEqual(2, nameMatchHandlers.Count);
            Assert.IsTrue(typeof(Handler1) == nameMatchHandlers[0].GetType());
            Assert.IsTrue(typeof(Handler2) == nameMatchHandlers[1].GetType());
        }

        [TestMethod]
        public void ShouldMatchPolicyByMethodName()
        {
            var policies = GetMultiplePolicySet();

            MethodImplementationInfo noMatchMember = GetMethodImplInfo<MatchesByMemberName>("NoMatch");
            MethodImplementationInfo nameMatchMember = GetMethodImplInfo<MatchesByMemberName>("NameMatch");
            List<ICallHandler> noMatchHandlers =
                new List<ICallHandler>(PolicyInjectionBehavior.CalculateHandlersFor(policies, noMatchMember, Container));
            List<ICallHandler> nameMatchHandlers =
                new List<ICallHandler>(PolicyInjectionBehavior.CalculateHandlersFor(policies, nameMatchMember, Container));

            Assert.AreEqual(0, noMatchHandlers.Count);
            Assert.AreEqual(1, nameMatchHandlers.Count);
            Assert.IsTrue(typeof(Handler2) == nameMatchHandlers[0].GetType());
        }

        [TestMethod]
        public void ShouldNotMatchPolicyWhenNoRulesMatch()
        {
            var policies = GetMultiplePolicySet();

            MethodImplementationInfo noMatchMember = GetMethodImplInfo<NoMatchAnywhere>("NoMatchHere");
            List<ICallHandler> noMatchHandlers =
                new List<ICallHandler>(PolicyInjectionBehavior.CalculateHandlersFor(policies, noMatchMember, Container));
            Assert.AreEqual(0, noMatchHandlers.Count);
        }

        [TestMethod]
        public void ShouldGetCorrectHandlersGivenAttributesOnInterfaceMethodsAfterAddingAttributeDrivenPolicy()
        {
            var policies = new List<InjectionPolicy>();

            List<ICallHandler> oneHandlers
                = new List<ICallHandler>(PolicyInjectionBehavior.CalculateHandlersFor(policies, GetMethodImplInfo<TwoType>("One"), Container));

            Assert.AreEqual(0, oneHandlers.Count);

            policies.Add(new AttributeDrivenPolicy());

            MethodImplementationInfo oneInfo = new MethodImplementationInfo(
                typeof(IOne).GetMethod("One"),
                typeof(TwoType).GetMethod("One"));

            oneHandlers
                = new List<ICallHandler>(PolicyInjectionBehavior.CalculateHandlersFor(policies, oneInfo, Container));

            Assert.AreEqual(2, oneHandlers.Count);
            Assert.IsTrue(oneHandlers[0] is MarkerCallHandler);
            Assert.IsTrue(oneHandlers[1] is MarkerCallHandler);

            Assert.AreEqual("IOneOne", ((MarkerCallHandler)oneHandlers[0]).HandlerName);
            Assert.AreEqual("MethodOneOverride", ((MarkerCallHandler)oneHandlers[1]).HandlerName);
        }

        [TestMethod]
        public void ShouldNotDuplicateHandlersWhenCreatingViaInterface()
        {
            Container
                .RegisterInstance<ICallHandler>("Handler1", new CallCountHandler())
                .RegisterInstance<ICallHandler>("Handler2", new CallCountHandler());

            RuleDrivenPolicy policy
                = new RuleDrivenPolicy("MatchesInterfacePolicy",
                    new IMatchingRule[] { new TypeMatchingRule("ITwo") },
                    new ICallHandler[] { new CallCountHandler(), new CallCountHandler() });

            var policies = new List<InjectionPolicy>() { policy };
            MethodImplementationInfo twoInfo = new MethodImplementationInfo(
                typeof(ITwo).GetMethod("Two"), typeof(TwoType).GetMethod("Two"));

            List<ICallHandler> handlrs
                = new List<ICallHandler>(PolicyInjectionBehavior.CalculateHandlersFor(policies, twoInfo, Container));
            Assert.AreEqual(2, handlrs.Count);
        }

        [TestMethod]
        public void HandlersOrderedProperly()
        {

            ICallHandler handler1 = new CallCountHandler();
            handler1.Order = 3;

            ICallHandler handler2 = new CallCountHandler();
            handler2.Order = 0;

            ICallHandler handler3 = new CallCountHandler();
            handler3.Order = 2;

            ICallHandler handler4 = new CallCountHandler();
            handler4.Order = 1;

            RuleDrivenPolicy policy
                = new RuleDrivenPolicy("MatchesInterfacePolicy",
                    new IMatchingRule[] { new TypeMatchingRule("ITwo") },
                    new ICallHandler[] { handler1, handler2, handler3, handler4 });

            var policies = new List<InjectionPolicy>() { policy };

            MethodImplementationInfo twoInfo = new MethodImplementationInfo(
                typeof(ITwo).GetMethod("Two"), typeof(TwoType).GetMethod("Two"));

            var handlers = PolicyInjectionBehavior.CalculateHandlersFor(policies, twoInfo, Container).ToList();

            Assert.AreEqual(handler4.Order, handlers[0].Order);
            Assert.AreEqual(handler3.Order, handlers[1].Order);
            Assert.AreEqual(handler1.Order, handlers[2].Order);
            Assert.AreEqual(handler2.Order, handlers[3].Order);
        }

        [TestMethod]
        public void HandlersOrderedProperlyUsingRelativeAndAbsoluteOrder()
        {
            //RuleDrivenPolicy policy
            //    = new RuleDrivenPolicy("MatchesInterfacePolicy",
            //        new IMatchingRule[] { new TypeMatchingRule("ITwo") },
            //        new string[] { "Handler1", "Handler2", "Handler3", "Handler4", "Handler5", "Handler6" });

            //ICallHandler handler1 = new CallCountHandler();
            //handler1.Order = 0;

            //ICallHandler handler2 = new CallCountHandler();
            //handler2.Order = 3;

            //ICallHandler handler3 = new CallCountHandler();
            //handler3.Order = 3;

            //ICallHandler handler4 = new CallCountHandler();
            //handler4.Order = 2;

            //ICallHandler handler5 = new CallCountHandler();
            //handler5.Order = 4;

            //ICallHandler handler6 = new CallCountHandler();
            //handler6.Order = 1;

            //Container
            //    .RegisterInstance<ICallHandler>("Handler1", handler1)
            //    .RegisterInstance<ICallHandler>("Handler2", handler2)
            //    .RegisterInstance<ICallHandler>("Handler3", handler3)
            //    .RegisterInstance<ICallHandler>("Handler4", handler4)
            //    .RegisterInstance<ICallHandler>("Handler5", handler5)
            //    .RegisterInstance<ICallHandler>("Handler6", handler6);

            //var policies = new List<InjectionPolicy>() { policy };

            //MethodImplementationInfo twoInfo = new MethodImplementationInfo(
            //    typeof(ITwo).GetMethod("Two"), typeof(TwoType).GetMethod("Two"));

            //List<ICallHandler> handlers
            //    = new List<ICallHandler>(PolicyInjectionBehavior.CalculateHandlersFor(policies, twoInfo, Container));

            //Assert.AreEqual(handler6.Order, handlers[0].Order);
            //Assert.AreEqual(handler4.Order, handlers[1].Order);
            //Assert.AreEqual(handler2.Order, handlers[2].Order);
            //Assert.AreEqual(handler3.Order, handlers[3].Order);
            //Assert.AreEqual(handler5.Order, handlers[4].Order);
            //Assert.AreEqual(handler1.Order, handlers[5].Order);
        }

        private List<InjectionPolicy> GetMultiplePolicySet()
        {
            RuleDrivenPolicy typeMatchPolicy
                = new RuleDrivenPolicy("MatchesType",
                    new IMatchingRule[] { new TypeMatchingRule(typeof(MatchesByType)) },
                    new ICallHandler[] { new Handler1() });

            RuleDrivenPolicy nameMatchPolicy
                = new RuleDrivenPolicy("MatchesName",
                    new IMatchingRule[] { new MemberNameMatchingRule("NameMatch") },
                    new ICallHandler[] { new Handler2() });

            return new List<InjectionPolicy> { typeMatchPolicy, nameMatchPolicy };
        }

        private MethodImplementationInfo GetMethodImplInfo<T>(string methodName)
        {
            return new MethodImplementationInfo(null,
                typeof(T).GetMethod(methodName));
        }
    }

    internal class MatchesByType
    {
        // Matches type policy
        public void NameDoesntMatch() { }

        // matches type & name policies
        public void NameMatch() { }
    }

    internal class MatchesByMemberName
    {
        public void NameMatch() { }

        public void NoMatch() { }
    }

    internal class NoMatchAnywhere
    {
        public void NoMatchHere() { }
    }

    public interface IOne
    {
        [MarkerCallHandler("IOneOne")]
        void One();
    }

    public interface ITwo
    {
        void Two();
    }

    public class OneType : IOne
    {
        public void MethodOne() { }

        public virtual void MethodTwo() { }

        public virtual void One() { }
    }

    public class TwoType : OneType, ITwo
    {
        public void BarOne() { }

        public override void MethodTwo() { }

        [MarkerCallHandler("MethodOneOverride")]
        public override void One() { }

        public void Two() { }
    }

    public class MarkerCallHandler : ICallHandler
    {
        private string handlerName;
        private int order = 0;

        public MarkerCallHandler(string handlerName)
        {
            this.handlerName = handlerName;
        }

        public string HandlerName
        {
            get { return handlerName; }
            set { handlerName = value; }
        }

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
            return getNext()(input, getNext);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
    public class MarkerCallHandlerAttribute : HandlerAttribute
    {
        private string handlerName;

        public MarkerCallHandlerAttribute(string handlerName)
        {
            this.handlerName = handlerName;
        }

        public override ICallHandler CreateHandler(IUnityContainer ignored)
        {
            return new MarkerCallHandler(this.handlerName);
        }
    }
}
