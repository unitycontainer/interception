

using System.Reflection;
using Unity.Interception.Utilities;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration
{
    internal static class IInterceptingProxyMethods
    {
        internal static MethodInfo AddInterceptionBehavior
        {
            get { return StaticReflection.GetMethodInfo<IInterceptingProxy>(ip => ip.AddInterceptionBehavior(null)); }
        }
    }
}
