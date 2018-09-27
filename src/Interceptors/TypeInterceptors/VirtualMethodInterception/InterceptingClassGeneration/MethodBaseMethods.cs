

using System;
using System.Reflection;
using Unity.Interception.Utilities;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration
{
    internal static class MethodBaseMethods
    {
        internal static MethodInfo GetMethodFromHandle
        {
            get
            {
                return StaticReflection.GetMethodInfo(
                    () => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle)));
            }
        }

        internal static MethodInfo GetMethodForGenericFromHandle
        {
            get
            {
                return StaticReflection.GetMethodInfo(
                    () => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle), default(RuntimeTypeHandle)));
            }
        }
    }
}
