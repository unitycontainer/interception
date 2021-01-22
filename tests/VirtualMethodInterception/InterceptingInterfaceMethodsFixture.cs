using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Interception;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.VirtualMethodInterception
{
    [TestClass]
    public class InterceptingInterfaceMethodsFixture
    {
        [TestMethod]
        public void ImplicitlyImplementedMethodsAreInterceptedIfVirtual()
        {
            InvokeCountHandler handler = new InvokeCountHandler();
            Interesting instance = WireupHelper.GetInterceptedInstance<Interesting>("DoSomethingInteresting", handler);

            instance.DoSomethingInteresting();

            Assert.IsTrue(instance.SomethingWasCalled);
            Assert.AreEqual(1, handler.Count);
        }
    }

    public interface IOne
    {
        void DoSomethingInteresting();
    }

    public class Interesting : IOne
    {
        public bool SomethingWasCalled;

        public virtual void DoSomethingInteresting()
        {
            SomethingWasCalled = true;
        }
    }
}
