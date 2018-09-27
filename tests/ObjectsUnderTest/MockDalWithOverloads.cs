

using System;
using Unity.Interception.PolicyInjection.MatchingRules;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    internal partial class MockDalWithOverloads
    {
        public int DoSomething(string s)
        {
            return 42;
        }

        [Tag("NullString")]
        public string DoSomething(int i)
        {
            return (i * 2).ToString();
        }
    }
}
