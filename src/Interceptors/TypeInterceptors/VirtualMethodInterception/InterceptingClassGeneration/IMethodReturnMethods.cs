

using System.Reflection;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.Utilities;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration
{
    internal static class IMethodReturnMethods
    {
        internal static MethodInfo GetException
        {
            get { return StaticReflection.GetPropertyGetMethodInfo((IMethodReturn imr) => imr.Exception); }
        }

        internal static MethodInfo GetReturnValue
        {
            get { return StaticReflection.GetPropertyGetMethodInfo((IMethodReturn imr) => imr.ReturnValue); }
        }

        internal static MethodInfo GetOutputs
        {
            get { return StaticReflection.GetPropertyGetMethodInfo((IMethodReturn imr) => imr.Outputs); }
        }
    }
}
