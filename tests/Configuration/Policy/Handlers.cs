using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Interception;

namespace Configuration
{
    public partial class PolicyFixture
    {
        [TestMethod("Add Rule By Name (missing)"), TestProperty(TEST, HANDLERS)]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void AddHandlerByNameMissing()
        {
            Container.Configure<Interception>()
                     .AddPolicy(PolicyName)
                        .AddCallHandler(Name);

            _ = Container.Resolve<InjectionPolicy>(PolicyName);
        }


        [TestMethod("Add Rule By Name"), TestProperty(TEST, HANDLERS)]
        public void AddHandlerByName()
        {
            Container.RegisterInstance(Name, handler)
                     .Configure<Interception>()
                        .AddPolicy(PolicyName)
                            .AddCallHandler(Name);

            // empty
            var policy = Container.Resolve<InjectionPolicy>(PolicyName);

            Assert.IsNotNull(policy);
            Assert.IsInstanceOfType(policy, typeof(RuleDrivenPolicy));
            Assert.AreEqual(PolicyName, policy.Name);
            Assert.IsFalse(policy.Matches(info));
        }


        [TestMethod("Add Rule By Instance"), TestProperty(TEST, HANDLERS)]
        public void AddHandlerByInstance()
        {
            Container.Configure<Interception>()
                        .AddPolicy(PolicyName)
                            .AddCallHandler(handler);

            // empty
            var policy = Container.Resolve<InjectionPolicy>(PolicyName);

            Assert.IsNotNull(policy);
            Assert.IsInstanceOfType(policy, typeof(RuleDrivenPolicy));
            Assert.AreEqual(PolicyName, policy.Name);
            Assert.IsFalse(policy.Matches(info));
        }
    }
}
