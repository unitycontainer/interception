using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Interception.Interceptors.InstanceInterceptors.TransparentProxyInterception;

namespace PolicyInjection
{
    public partial class PolicyInjectionWithGenericMethodsFixture
    {
        [TestMethod]
        public void TransparentProxyCanInterceptNonGenericMethod()
        {
            CanInterceptNonGenericMethod<TransparentProxyInterceptor>();
        }

        [TestMethod]
        public void TransparentProxyCanInterceptGenericMethod()
        {
            CanInterceptGenericMethod<TransparentProxyInterceptor>();
        }
    }
}
