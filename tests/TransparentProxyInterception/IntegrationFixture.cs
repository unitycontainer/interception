

using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.TransparentProxyInterception;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.TransparentProxyInterception
{
    [TestClass]
    public class IntegrationFixture
    {
        [TestMethod]
        public void CanInterceptGenericMethodWithHandlerAttributeThroughInterface()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.RegisterType<IInterfaceWithGenericMethod, ClassWithGenericMethod>(
                new Interceptor(new TransparentProxyInterceptor()));

            var instance = container.Resolve<IInterfaceWithGenericMethod>();

            var result = instance.DoSomething<int>();

            Assert.AreEqual(0, result);
        }
    }
}
