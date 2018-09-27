

using System;
using System.Reflection;
using Unity.Interception.Interceptors;
using Unity.Interception.PolicyInjection;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.Utilities;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.VirtualMethodInterception
{
    internal class WireupHelper
    {
        internal static T GetInterceptingInstance<T>(params object[] ctorValues)
        {
            Type typeToIntercept = typeof(T);
            if (typeToIntercept.IsGenericType)
            {
                typeToIntercept = typeToIntercept.GetGenericTypeDefinition();
            }

            global::Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration.InterceptingClassGenerator generator = new global::Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration.InterceptingClassGenerator(typeToIntercept);
            Type generatedType = generator.GenerateType();

            if (generatedType.IsGenericTypeDefinition)
            {
                generatedType = generatedType.MakeGenericType(typeof(T).GetGenericArguments());
            }

            return (T)Activator.CreateInstance(generatedType, ctorValues);
        }

        internal static T GetInterceptedInstance<T>(string methodName, ICallHandler handler)
        {
            MethodInfo method = typeof(T).GetMethod(methodName);

            T instance = GetInterceptingInstance<T>();

            PipelineManager manager = new PipelineManager();
            manager.SetPipeline(method, new HandlerPipeline(Sequence.Collect(handler)));

            IInterceptingProxy pm = (IInterceptingProxy)instance;
            pm.AddInterceptionBehavior(new PolicyInjectionBehavior(manager));

            return instance;
        }
    }
}
