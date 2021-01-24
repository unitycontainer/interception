using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration;

namespace Unity.Interception
{
    /// <summary>
    /// An instance interceptor that works by generating a
    /// proxy class on the fly for a single interface.
    /// </summary>
    public class InterfaceInterceptor : IInstanceInterceptor
    {
        private static readonly Dictionary<GeneratedTypeKey, Type> InterceptorClasses =
            new Dictionary<GeneratedTypeKey, Type>(new GeneratedTypeKey.GeneratedTypeKeyComparer());

        #region IInstanceInterceptor Members

        /// <summary>
        /// Can this interceptor generate a proxy for the given type?
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if interception is possible, false if not.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanIntercept(Type type) => type.IsInterface;


        /// <summary>
        /// Returns a sequence of methods on the given type that can be
        /// intercepted.
        /// </summary>
        /// <param name="interceptedType">Type that was specified when this interceptor
        /// was created (typically an interface).</param>
        /// <param name="implementationType">The concrete type of the implementing object.</param>
        /// <returns>Sequence of <see cref="MethodInfo"/> objects.</returns>
        public IEnumerable<MethodImplementationInfo> GetInterceptableMethods(Type interceptedType, Type implementationType)
        {
            if (interceptedType.IsInterface && implementationType.IsInterface
                && interceptedType.IsAssignableFrom(implementationType))
            {
                var methods = interceptedType.GetMethods();
                for (int i = 0; i < methods.Length; ++i)
                {
                    yield return new MethodImplementationInfo(methods[i], methods[i]);
                }
            }
            else
            {
                InterfaceMapping mapping = implementationType.GetInterfaceMap(interceptedType);
                for (int i = 0; i < mapping.InterfaceMethods.Length; ++i)
                {
                    yield return new MethodImplementationInfo(mapping.InterfaceMethods[i], mapping.TargetMethods[i]);
                }
            }

            foreach (var type in interceptedType.GetInterfaces())
            {
                foreach (var info in GetInterceptableMethods(type, implementationType))
                {
                    yield return info;
                }
            }
        }


        /// <summary>
        /// Create a proxy object that provides interception for <paramref name="target"/>.
        /// </summary>
        /// <param name="type">Type to generate the proxy of.</param>
        /// <param name="target">Object to create the proxy for.</param>
        /// <param name="interfaces">Additional interfaces the proxy must implement.</param>
        /// <returns>The proxy object.</returns>
        public IInterceptingProxy CreateProxy(Type type, object target, params Type[] interfaces)
        {
            Type interceptorType;
            Type typeToProxy = type;
            bool genericType = false;

            if (type.IsGenericType)
            {
                typeToProxy = type.GetGenericTypeDefinition();
                genericType = true;
            }

            GeneratedTypeKey key = new GeneratedTypeKey(typeToProxy, interfaces);
            lock (InterceptorClasses)
            {
                if (!InterceptorClasses.TryGetValue(key, out interceptorType))
                {
                    InterfaceInterceptorClassGenerator generator =
                        new InterfaceInterceptorClassGenerator(typeToProxy, interfaces);
                    interceptorType = generator.CreateProxyType();
                    InterceptorClasses[key] = interceptorType;
                }
            }

            if (genericType)
            {
                interceptorType = interceptorType.MakeGenericType(type.GetGenericArguments());
            }
            return (IInterceptingProxy)interceptorType.GetConstructors()[0].Invoke(new[] { target, type });
        }

        #endregion


        #region ISequenceSegment

        public ISequenceSegment? Next { get; set; }

        #endregion
    }
}
