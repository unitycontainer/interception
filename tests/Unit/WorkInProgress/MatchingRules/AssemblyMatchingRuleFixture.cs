

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Interception.PolicyInjection.MatchingRules;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules
{
    [TestClass]
    public partial class AssemblyMatchingRuleFixture
    {
        private MethodBase objectToStringMethod;

        [TestInitialize]
        public void TestInitialize()
        {
            objectToStringMethod = typeof(object).GetMethod("ToString");
        }

        [TestMethod]
        public void CanMatchAssemblyNameByNameOnly()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanExplicitlyDenyMatchOnVersion()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=1.2.3.4, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }
    }
}
