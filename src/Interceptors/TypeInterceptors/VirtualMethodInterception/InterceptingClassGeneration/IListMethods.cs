

using System.Collections;
using System.Reflection;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration
{
    internal static class IListMethods
    {
        internal static MethodInfo GetItem => typeof(IList).GetProperty("Item")?.GetGetMethod();
    }
}
