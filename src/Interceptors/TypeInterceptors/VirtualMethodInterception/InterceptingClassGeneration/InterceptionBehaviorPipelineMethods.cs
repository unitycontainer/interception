

using System.Reflection;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Utilities;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration
{
    internal static class InterceptionBehaviorPipelineMethods
    {
        internal static ConstructorInfo Constructor
        {
            get { return StaticReflection.GetConstructorInfo(() => new InterceptionBehaviorPipeline()); }
        }

        internal static MethodInfo Add
        {
            get { return StaticReflection.GetMethodInfo((InterceptionBehaviorPipeline pip) => pip.Add(null)); }
        }

        internal static MethodInfo Invoke
        {
            get { return StaticReflection.GetMethodInfo((InterceptionBehaviorPipeline pip) => pip.Invoke(null, null)); }
        }
    }
}
