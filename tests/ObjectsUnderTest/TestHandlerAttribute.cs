using Unity;
using Unity.Interception;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    public class TestHandlerAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new TestHandler();
        }
    }
}