using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Interception;
using Unity.Interception.Interceptors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Configuration
{
    public partial class PolicyFixture
    {
        [TestMethod("No rules, no match"), TestProperty(TEST, MATCHING)]
        public void ShouldNotMatchWithNoContainedRules()
        {
            Container.Configure<Interception>()
                        .AddPolicy(PolicyName);

            var policy = Container.Resolve<InjectionPolicy>(PolicyName);
            
            Assert.IsFalse(policy.Matches(info));
        }

        [TestMethod("Matches when appropriate"), TestProperty(TEST, MATCHING)]
        public void ShouldMatchWithMatchingTypeRule()
        {
            Container.Configure<Interception>()
                        .AddPolicy(PolicyName)
                            .AddMatchingRule(rule);


            var policy = Container.Resolve<InjectionPolicy>(PolicyName);

            Assert.IsTrue(policy.Matches(info));
        }

        [TestMethod("Matches all or nothing"), TestProperty(TEST, MATCHING)]
        public void ShouldNotMatchWhenOneRuleDoesntMatch()
        {
            Container.Configure<Interception>()
                        .AddPolicy(PolicyName)
                            .AddMatchingRule(rule)
                            .AddMatchingRule(new TypeMatchingRule(typeof(Interception)));

            var policy = Container.Resolve<InjectionPolicy>(PolicyName);

            Assert.IsFalse(policy.Matches(info));
        }


        [TestMethod("No handlers by default"), TestProperty(TEST, MATCHING)]
        public void ShouldHaveNoHandlersWhenPolicyDoesntMatch()
        {
            Container.Configure<Interception>()
                        .AddPolicy(PolicyName);

            var memberHandlers = Container.Resolve<InjectionPolicy>(PolicyName)
                                          .GetHandlersFor(info, Container).ToArray();
            
            Assert.AreEqual(0, memberHandlers.Length);
        }


        [TestMethod("Same Order With GetHandlersFor"), TestProperty(TEST, MATCHING)]
        public void ShouldGetHandlersInOrderWithGetHandlersFor()
        {
            Container.Configure<Interception>()
                        .AddPolicy(PolicyName)
                            .AddMatchingRule(rule)
                            .AddCallHandler("handler1")
                            .AddCallHandler("handler2")
                            .AddCallHandler("handler3");

            var expectedHandlers = Container.Resolve<ICallHandler[]>();
            var actualHandlers = Container.Resolve<InjectionPolicy>(PolicyName)
                                          .GetHandlersFor(info, Container)
                                          .ToArray();
            
            CollectionAssert.AreEqual(expectedHandlers, actualHandlers, new TypeComparer());
        }


        [TestMethod("Match PropertyGet"), TestProperty(TEST, MATCHING)]
        public void ShouldBeAbleToMatchPropertyGet()
        {
            Container.Configure<Interception>()
                        .AddPolicy(PolicyName)
                            .AddMatchingRule(new MemberNameMatchingRule("get_Container"))
                            .AddCallHandler("handler1")
                            .AddCallHandler("handler2")
                            .AddCallHandler("handler3");

            PropertyInfo property = GetType().GetProperty(nameof(Container));
            MethodImplementationInfo getMethod = new MethodImplementationInfo(null, property.GetGetMethod());
            
            var policy = Container.Resolve<InjectionPolicy>(PolicyName);


            var expectedHandlers = Container.Resolve<ICallHandler[]>();
            var actualHandlers = policy.GetHandlersFor(getMethod, Container)
                                       .ToArray();

            CollectionAssert.AreEqual(expectedHandlers, actualHandlers, new TypeComparer());
        }

    }
}
