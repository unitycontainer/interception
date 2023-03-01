﻿

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
        private MethodBase objectCtor;
        private MethodBase stringCopyToMethod;

        [TestInitialize]
        public void TestInitialize()
        {
            objectToStringMethod = typeof(object).GetMethod("ToString");
            objectCtor = typeof(object).GetConstructor(new Type[0]);
            stringCopyToMethod = typeof(string).GetMethod("CopyTo");
        }

        [TestMethod]
        public void MatchIsDeniedWhenParamterValuesCountDiffers()
        {
            List<string> oneParam = new List<string>();
            oneParam.Add("one");

            MethodSignatureMatchingRule matchingRule = new MethodSignatureMatchingRule(oneParam);
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchOnParameterlessMethods()
        {
            List<string> parameterLess = new List<string>();
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
            List<string> parametersForCopyToMethod = new List<string>
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
