

using System;
using System.Reflection;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.Utilities;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration
{
    /// <summary>
    /// MethodInfo objects for the methods we need to generate
    /// calls to on IMethodInvocation.
    /// </summary>
    internal static class IMethodInvocationMethods
    {
        internal static MethodInfo CreateExceptionMethodReturn
        {
            get { return StaticReflection.GetMethodInfo((IMethodInvocation mi) => mi.CreateExceptionMethodReturn(default(Exception))); }
        }

        internal static MethodInfo CreateReturn => typeof(IMethodInvocation).GetMethod("CreateMethodReturn");

        internal static MethodInfo GetArguments
        {
            get { return StaticReflection.GetPropertyGetMethodInfo((IMethodInvocation mi) => mi.Arguments); }
        }
    }
}
