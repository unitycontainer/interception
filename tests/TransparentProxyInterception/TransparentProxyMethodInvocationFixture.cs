using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Interception.Interceptors.InstanceInterceptors.TransparentProxyInterception;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    /// <summary>
    /// Tests for the TransparentProxyMethodInvocation class, which wraps
    /// a IMethodCallMessage in an IMethodInvocation implementation.
    /// </summary>
    [TestClass]
    public class TransparentProxyMethodInvocationFixture
    {
        #region Test Methods

        [TestMethod]
        public void ShouldBeCreatable()
        {
            var methodInfo = GetTargetMethodInfo("FirstTarget");
            var target = new InvocationTarget();
            var invocation = GetInvocation(methodInfo, target);

            Assert.IsNotNull(invocation);
        }

        [TestMethod]
        public void ShouldMapInputsCorrectly()
        {
            var methodInfo = GetTargetMethodInfo("FirstTarget");
            var target = new InvocationTarget();
            var invocation = GetInvocation(methodInfo, target);

            Assert.AreEqual(2, invocation.Inputs.Count);
            Assert.AreEqual(1, invocation.Inputs[0]);
            Assert.AreEqual("two", invocation.Inputs[1]);
            Assert.AreEqual("two", invocation.Inputs["two"]);
            Assert.AreEqual(1, invocation.Inputs["one"]);
            Assert.AreEqual(methodInfo, invocation.MethodBase);
            Assert.AreSame(target, invocation.Target);
        }

        [TestMethod]
        public void ShouldBeAbleToAddToContext()
        {
            var methodInfo = GetTargetMethodInfo("FirstTarget");
            var target = new InvocationTarget();
            var invocation = GetInvocation(methodInfo, target);

            invocation.InvocationContext["firstItem"] = 1;
            invocation.InvocationContext["secondItem"] = "hooray!";

            Assert.AreEqual(2, invocation.InvocationContext.Count);
            Assert.AreEqual(1, invocation.InvocationContext["firstItem"]);
            Assert.AreEqual("hooray!", invocation.InvocationContext["secondItem"]);
        }

        [TestMethod]
        public void ShouldBeAbleToChangeInputs()
        {
            var methodInfo = GetTargetMethodInfo("FirstTarget");
            var target = new InvocationTarget();
            var invocation = GetInvocation(methodInfo, target);

            Assert.AreEqual(1, invocation.Inputs["one"]);
            invocation.Inputs["one"] = 42;
            Assert.AreEqual(42, invocation.Inputs["one"]);
        }

        #endregion

        #region Helper factories

        private MethodInfo GetTargetMethodInfo(string methodName)
        {
            return (MethodInfo)(typeof(InvocationTarget).GetMember(methodName)[0]);
        }

        private IMethodInvocation GetInvocation(MethodInfo targetMethod,
                                                InvocationTarget target)
        {
            return new TransparentProxyMethodInvocation(targetMethod, target, 1, "two");
        }

        #endregion
    }

    public class InvocationTarget : MarshalByRefObject
    {
        public string FirstTarget(int one,
                                  string two)
        {
            return "Boo!";
        }
    }
}
