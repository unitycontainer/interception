

using System.Reflection;
using Unity.Interception.Utilities;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration
{
    internal static class ObjectMethods
    {
        // Zero argument constructor
        internal static ConstructorInfo Constructor { get { return StaticReflection.GetConstructorInfo(() => new object()); } }
    }
}
