using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Interception.PolicyInjection.MatchingRules;

namespace Unit.Tests
{
    /// <summary>
    /// Tests for the MatchingRuleSet class
    /// </summary>
    [TestClass]
    public class MatchingRuleSetFixture
    {
        [TestMethod]
        public void ShouldNotMatchWithNoContainedRules()
        {
            MatchingRuleSet ruleSet = new MatchingRuleSet();

            MethodBase member = GetType().GetMethod(nameof(ShouldNotMatchWithNoContainedRules));
            Assert.IsFalse(ruleSet.Matches(member));
        }

        [TestMethod]
        public void ShouldMatchWithMatchingTypeRule()
        {
            MatchingRuleSet ruleSet = new MatchingRuleSet
            {
                new TypeMatchingRule(typeof(MatchingRuleSetFixture))
            };
            MethodBase member = GetType().GetMethod(nameof(ShouldMatchWithMatchingTypeRule));
            Assert.IsTrue(ruleSet.Matches(member));
        }

        [TestMethod]
        public void ShouldNotMatchWhenOneRuleDoesntMatch()
        {
            MethodBase member = GetType().GetMethod(nameof(ShouldNotMatchWhenOneRuleDoesntMatch));
            MatchingRuleSet ruleSet = new MatchingRuleSet
            {
                new TypeMatchingRule(typeof(MatchingRuleSetFixture))
            };
            Assert.IsTrue(ruleSet.Matches(member));

            ruleSet.Add(new MemberNameMatchingRule("ThisMethodDoesntExist"));
            Assert.IsFalse(ruleSet.Matches(member));
        }
    }
}
