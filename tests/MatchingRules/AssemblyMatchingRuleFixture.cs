

using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Interception.PolicyInjection.MatchingRules;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules
{
    [TestClass]
    public partial class AssemblyMatchingRuleFixture
    {
        private MethodBase _objectToStringMethod;

        [TestInitialize]
        public void TestInitialize()
        {
            _objectToStringMethod = typeof(object).GetMethod("ToString");
        }

        [TestMethod]
        public void CanMatchAssemblyNameByNameOnly()
        {
            var name = typeof(object).Assembly.GetName().Name;
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule(name);
            Assert.IsTrue(matchingRule.Matches(_objectToStringMethod));
        }

        [TestMethod]
        public void CanExplicitlyDenyMatchOnVersion()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=1.2.3.4, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            Assert.IsFalse(matchingRule.Matches(_objectToStringMethod));
        }
    }
}
