

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Interception.PolicyInjection.MatchingRules;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules
{
    [TestClass]
    public class MethodSignatureMatchingRuleFixture
    {
        private MethodBase objectToStringMethod;
        private MethodBase stringCopyToMethod;

        [TestInitialize]
        public void TestInitialize()
        {
            objectToStringMethod = typeof(object).GetMethod("ToString");
            stringCopyToMethod = typeof(string).GetMethod("CopyTo", new Type[] { typeof(int), typeof(char[]), typeof(int), typeof(int) });
        }

        [TestMethod]
        public void MatchIsDeniedWhenParamterValuesCountDiffers()
        {
            List<string> oneParam = new()
            {
                "one"
            };

            MethodSignatureMatchingRule matchingRule = new MethodSignatureMatchingRule(oneParam);
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchOnParameterlessMethods()
        {
            List<string> parameterLess = new();
            MethodSignatureMatchingRule matchingRule = new MethodSignatureMatchingRule(parameterLess);
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchOnMultipleParameterTypes()
        {
            List<string> parametersForCopyToMethod = new()
            {
                "System.Int32",
                "System.Char[]",
                "System.Int32",
                "System.Int32"
            };

            MethodSignatureMatchingRule matchingRule = new MethodSignatureMatchingRule(parametersForCopyToMethod);
            Assert.IsTrue(matchingRule.Matches(stringCopyToMethod));
        }

        [TestMethod]
        public void MatchIsDeniedWhenASingleParameterIsWrong()
        {
            List<string> parametersForCopyToMethod = new()
            {
                "System.Int32",
                "System.Char[]",
                "System.NotAnInt32",
                "System.Int32"
            };

            MethodSignatureMatchingRule matchingRule = new MethodSignatureMatchingRule(parametersForCopyToMethod);
            Assert.IsFalse(matchingRule.Matches(stringCopyToMethod));
        }
    }
}
