

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    /// <summary>
    /// Summary description for CodeplexIssuesFixture
    /// </summary>
    [TestClass]
    public class CodeplexIssuesFixture
    {
        public interface IRepository { }
        public class TestRepository : IRepository { }
        public class TestService
        {
            public TestService(IRepository repository)
            {
            }
        }

        [TestMethod]
        public void DependenciesAndInterceptionMixProperly()
        {
            var container = new UnityContainer()
                .AddNewExtension<Interception>()
                .RegisterType<IRepository, TestRepository>()
                .RegisterType<TestService>(
                    new Interceptor<VirtualMethodInterceptor>());

            var svc1 = container.Resolve<TestService>();
            var svc2 = container.Resolve<TestService>();

            Assert.AreNotSame(svc1, svc2);
            Assert.IsNotNull(svc1 as IInterceptingProxy);
            Assert.IsNotNull(svc2 as IInterceptingProxy);
        }
    }
}
