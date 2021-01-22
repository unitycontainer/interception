using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Interception;
using Unity.Interception.PolicyInjection.Policies;

namespace Configuration
{
    public partial class PolicyFixture
    {
        [TestMethod("Rule By Name (missing)"), TestProperty(TEST, RULES)]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void AddRuleByNameMissing()
        {
            Container.Configure<Interception>()
                     .AddPolicy(PolicyName)
                        .AddMatchingRule(Name);

            _ = Container.Resolve<InjectionPolicy>(PolicyName);
        }


        [TestMethod("Rule By Name"), TestProperty(TEST, RULES)]
        public void AddRuleByName()
        {
            Container.RegisterInstance(Name, rule)
                     .Configure<Interception>()
                        .AddPolicy(PolicyName)
                            .AddMatchingRule(Name);

            // empty
            var policy = Container.Resolve<InjectionPolicy>(PolicyName);

            Assert.IsNotNull(policy);
            Assert.IsInstanceOfType(policy, typeof(RuleDrivenPolicy));
            Assert.AreEqual(PolicyName, policy.Name);
            Assert.IsTrue(policy.Matches(method));
        }


        [TestMethod("Rule By Instance"), TestProperty(TEST, RULES)]
        public void AddRuleByInstance()
        {
            Container.Configure<Interception>()
                        .AddPolicy(PolicyName)
                            .AddMatchingRule(rule);
            // empty
            var policy = Container.Resolve<InjectionPolicy>(PolicyName);

            Assert.IsNotNull(policy);
            Assert.IsInstanceOfType(policy, typeof(RuleDrivenPolicy));
            Assert.AreEqual(PolicyName, policy.Name);
            Assert.IsTrue(policy.Matches(method));
        }
    }
}
