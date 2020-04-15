﻿using System;
using Unity.Interception.PolicyInjection.Policies;

namespace Unity.Interception.Tests
{
    public interface IDal
    {
        void Deposit(double amount);
        void Withdraw(double amount);
    }

    public interface IMonitor
    {
        void Log(string message);
    }

    public partial class MockDal : IDal, IMonitor
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

#pragma warning disable IDE0060 // Remove unused parameter
        public int DoSomething(string x)
        {
            if (throwException)
            {
                throw new InvalidOperationException("Catastrophic");
            }
            return 42;
        }
#pragma warning restore IDE0060 // Remove unused parameter

        #region IDal Members

        public void Deposit(double amount)
        {
        }

        public void Withdraw(double amount)
        {
        }

        #endregion

        #region IMonitor Members

        public void Log(string message)
        {
        }

        #endregion

        [ApplyNoPolicies]
        public string SomethingCritical()
        {
            return "Don't intercept me";
        }
    }
}