

using System;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Interception.PolicyInjection.Policies;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    [ApplyNoPolicies]
    internal partial class CriticalFakeDal
    {
        private bool throwException;
        private double balance = 0.0;

        public bool ThrowException
        {
            get { return throwException; }
            set { throwException = value; }
        }

        public double Balance
        {
            get { return balance; }
            set { balance = value; }
        }

        public int DoSomething(string x)
        {
            if (throwException)
            {
                throw new InvalidOperationException("Catastrophic");
            }
            return 42;
        }

        public string SomethingCritical()
        {
            return "Don't intercept me";
        }
    }
}
