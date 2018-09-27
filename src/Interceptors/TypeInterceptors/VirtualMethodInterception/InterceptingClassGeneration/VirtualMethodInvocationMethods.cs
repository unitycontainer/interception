

using System.Reflection;
using Unity.Interception.Utilities;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration
{
    internal static class VirtualMethodInvocationMethods
    {
        internal static ConstructorInfo VirtualMethodInvocation
        {
            get
            {
                return StaticReflection.GetConstructorInfo(
                    () => new VirtualMethodInvocation(default(object), default(MethodBase)));
            }
        }
    }
}
