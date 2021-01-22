using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity;
using Unity.Interception;
using Unity.Interception.PolicyInjection.Policies;

namespace Configuration
{
    public partial class PolicyFixture
    {

        [TestMethod("AttributeDrivenPolicy configured by default"), TestProperty(TEST, "Defaults")]
        public void Baseline()
        {
            // empty
            var policies = Container.Resolve<InjectionPolicy[]>();

            Assert.AreEqual(1, policies.Length);
            Assert.IsInstanceOfType(policies[0], typeof(AttributeDrivenPolicy));
        }


        [TestMethod("Empty Rule"), TestProperty(TEST, EMPTY)]
        public void CanSetUpEmptyRule()
        {
            // empty
            Container
                .Configure<Interception>()
                    .AddPolicy(PolicyName);

            var policies = Container.Resolve<InjectionPolicy[]>();

            Assert.AreEqual(2, policies.Length);
            Assert.IsInstanceOfType(policies[0], typeof(AttributeDrivenPolicy));
            Assert.IsInstanceOfType(policies[1], typeof(RuleDrivenPolicy));
            Assert.AreEqual(PolicyName, policies[1].Name);
        }


        [TestMethod("Multiple Empty Rules"), TestProperty(TEST, EMPTY)]
        public void CanSetUpSeveralEmptyRules()
        {
            // empty
            Container
                .Configure<Interception>()
                    .AddPolicy(PolicyName).Interception
                    .AddPolicy("policy2");

            List<InjectionPolicy> policies
                = new List<InjectionPolicy>(Container.ResolveAll<InjectionPolicy>());

            Assert.AreEqual(3, policies.Count);
            Assert.IsInstanceOfType(policies[0], typeof(AttributeDrivenPolicy));
            Assert.IsInstanceOfType(policies[1], typeof(RuleDrivenPolicy));
            Assert.AreEqual(PolicyName, policies[1].Name);
            Assert.IsInstanceOfType(policies[2], typeof(RuleDrivenPolicy));
            Assert.AreEqual("policy2", policies[2].Name);
        }
    }
}
