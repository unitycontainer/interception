

using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Interception.PolicyInjection.MatchingRules;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules
{
    [TestClass]
    public class ReturnTypeMatchingRuleFixture
    {
        private MethodBase _objectToStringMethod;
        private MethodBase _objectCtor;
        private MethodBase _stringGetHashCode;

        [TestInitialize]
        public void TestInitialize()
        {
            _objectToStringMethod = typeof(object).GetMethod("ToString");
            _stringGetHashCode = typeof(string).GetMethod("GetHashCode", Array.Empty<Type>());
            _objectCtor = typeof(object).GetConstructor(new Type[0]);
        }

        [TestMethod]
        public void MatchIsDeniedWhenReturnTypeNameDiffers()
        {
            IMatchingRule matchingRule = new ReturnTypeMatchingRule("System.wichReturnType?");
            Assert.IsFalse(matchingRule.Matches(_objectToStringMethod));
        }

        [TestMethod]
        public void MatchIsAcceptedWhenReturnTypeNameIsExactMatch()
        {
            IMatchingRule matchingRule = new ReturnTypeMatchingRule("System.String");
            Assert.IsTrue(matchingRule.Matches(_objectToStringMethod));
        }

        [TestMethod]
        public void MatchIsDeniedWhenReturnTypeIsSpecifiedButNoReturnTypeExistsOnMethodBase()
        {
            IMatchingRule matchingRule = new ReturnTypeMatchingRule("void");
            Assert.IsFalse(matchingRule.Matches(_objectCtor));
        }

        [TestMethod]
        public void MatchIsAcceptedWhenReturnTypeIsVoidAndMethodReturnsVoid()
        {
            IMatchingRule matchingRule = new ReturnTypeMatchingRule("System.Int32");
            Assert.IsTrue(matchingRule.Matches(_stringGetHashCode));
        }

        [TestMethod]
        public void MatchIsAcceptedForTypeNameWithoutNamespace()
        {
            IMatchingRule matchingRule = new ReturnTypeMatchingRule("string", true);
            Assert.IsTrue(matchingRule.Matches(_objectToStringMethod));
        }
    }
}
