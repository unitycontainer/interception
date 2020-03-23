using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;
using Unity.Interception.Tests;

namespace Issue.Tests
{
    /// <summary>
    /// Codeplex Issues Fixture
    /// </summary>
    [TestClass]
    public class Codeplex : TestFixtureBase
    {
        [TestMethod]
        public void DependenciesAndInterceptionMixProperly()
        {
            Container
                .RegisterType<IRepository, TestRepository>()
                .RegisterType<TestService>(new Interceptor<VirtualMethodInterceptor>());

            var svc1 = Container.Resolve<TestService>();
            var svc2 = Container.Resolve<TestService>();

            Assert.AreNotSame(svc1, svc2);
            Assert.IsNotNull(svc1 as IInterceptingProxy);
            Assert.IsNotNull(svc2 as IInterceptingProxy);
        }

        #region Test Data

        public interface IRepository { }
        public class TestRepository : IRepository { }
        public class TestService
        {
            public TestService(IRepository repository)
            {
            }
        }


        #endregion
    }
}
