using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Interception;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;
using Unity.Interception.Tests;

namespace Standalone.Tests
{
    [TestClass]
    public class AddInterfaceThroughProxy
    {
        [TestMethod]
        public void CanProxyWithBehaviorThatAddsInterface()
        {
            // Arrange
            var target = new MockDal();

            // Act
            var proxied = Intercept.ThroughProxy<IDal>(target, new InterfaceInterceptor(), new[] { new AdditionalInterfaceBehavior() });

            // Validate
            Assert.IsNotNull(proxied);
        }

        [TestMethod]
        public void BehaviorAddsInterface()
        {
            // Arrange
            var target = new MockDal();

            // Act
            var proxied = Intercept.ThroughProxy<IDal>(target, new InterfaceInterceptor(), new[] { new AdditionalInterfaceBehavior() });

            // Validate
            Assert.IsNotNull(proxied as IAdditionalInterface);
        }

        [TestMethod]
        public void CanInvokeMethodAddedByBehavior()
        {
            // Act
            var proxied = Intercept.NewInstance<MockDal>( new VirtualMethodInterceptor(), new[] { new AdditionalInterfaceBehavior() });

            // Validate
            Assert.AreEqual(10, ((IAdditionalInterface)proxied).DoNothing());
        }

        [TestMethod]
        public void CanManuallyAddAdditionalInterface()
        {
            // Arrange
            var target = new MockDal();

            // Act
            var proxied = Intercept.ThroughProxyWithAdditionalInterfaces<IDal>(target, new InterfaceInterceptor(), new[] { new AdditionalInterfaceBehavior(false) }, 
                                                                                                                   new[] { typeof(IAdditionalInterface) });

            // Validate
            Assert.IsNotNull(proxied as IAdditionalInterface);
        }

        [TestMethod]
        public void CanInvokeMethodOnManuallyAddedInterface()
        {
            // Arrange
            var target = new MockDal();

            // Act
            var proxied = Intercept.ThroughProxyWithAdditionalInterfaces<IDal>(target, new InterfaceInterceptor(), new[] { new AdditionalInterfaceBehavior(false) },
                                                                                                                   new[] { typeof(IAdditionalInterface) });
            // Validate
            Assert.AreEqual(10, ((IAdditionalInterface)proxied).DoNothing());
        }
    }
}
