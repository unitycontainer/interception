// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Interception.ContainerIntegration.ObjectBuilder;
using Unity.Interception.Interceptors;
using Unity.Interception.Interceptors.InstanceInterceptors;
using Unity.Interception.Interceptors.TypeInterceptors;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;
using Unity.Interception.Utilities;
using Unity.Policy;

namespace Unity.Interception.ContainerIntegration
{
    /// <summary>
    /// Stores information about the <see cref="IInterceptor"/> to be used to intercept an object and
    /// configures a container accordingly.
    /// </summary>
    /// <seealso cref="IInterceptionBehavior"/>
    public class Interceptor : InterceptionMember
    {
        private readonly IInterceptor _interceptor;
        private readonly Type _type;
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interceptor"/> class with an interceptor instance.
        /// </summary>
        /// <param name="interceptor">The <see cref="IInterceptor"/> to use.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptor"/> is
        /// <see langword="null"/>.</exception>
        public Interceptor(IInterceptor interceptor)
        {
            _interceptor = interceptor ?? throw new ArgumentNullException(nameof(interceptor));
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Interceptor"/> class with a given
        /// name and type that will be resolved to provide interception.
        /// </summary>
        /// <param name="interceptorType">Type of the interceptor</param>
        /// <param name="name">name to use to resolve.</param>
        public Interceptor(Type interceptorType, string name)
        {
            Guard.TypeIsAssignable(typeof(IInterceptor), interceptorType ?? 
                  throw new ArgumentNullException(nameof(interceptorType)), 
                                                  nameof(interceptorType));

            _type = interceptorType;
            _name = name;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Interceptor"/> class with
        /// a given type that will be resolved to provide interception.
        /// </summary>
        /// <param name="interceptorType">Type of the interceptor.</param>
        public Interceptor(Type interceptorType)
            : this(interceptorType, null)
        {
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the container to use the represented 
        /// <see cref="IInterceptor"/> for the supplied parameters.
        /// </summary>
        /// <param name="serviceType">Interface being registered.</param>
        /// <param name="implementationType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            if (IsInstanceInterceptor)
            {
                var policy = CreateInstanceInterceptionPolicy();
                policies.Set(serviceType, name, typeof(IInstanceInterceptionPolicy), policy);
                policies.Clear(serviceType, name, typeof(ITypeInterceptionPolicy));
            }
            else
            {
                var policy = CreateTypeInterceptionPolicy();
                policies.Set(serviceType, name, typeof(ITypeInterceptionPolicy), policy);
                policies.Clear(serviceType, name, typeof(IInstanceInterceptionPolicy));
            }
        }

        public override bool BuildRequired => _type == typeof(VirtualMethodInterceptor);

        private bool IsInstanceInterceptor
        {
            get
            {
                if (_interceptor != null)
                {
                    return _interceptor is IInstanceInterceptor;
                }
                return typeof(IInstanceInterceptor).IsAssignableFrom(_type);
            }
        }

        private IInstanceInterceptionPolicy CreateInstanceInterceptionPolicy()
        {
            if (_interceptor != null)
            {
                return new FixedInstanceInterceptionPolicy((IInstanceInterceptor)_interceptor);
            }
            return new ResolvedInstanceInterceptionPolicy(_type, _name);
        }

        private ITypeInterceptionPolicy CreateTypeInterceptionPolicy()
        {
            if (_interceptor != null)
            {
                return new FixedTypeInterceptionPolicy((ITypeInterceptor)_interceptor);
            }
            return new ResolvedTypeInterceptionPolicy(_type, _name);
        }
    }

    /// <summary>
    /// Generic version of <see cref="Interceptor"/> that lets you specify an interceptor
    /// type using generic syntax.
    /// </summary>
    /// <typeparam name="TInterceptor">Type of interceptor</typeparam>
    public class Interceptor<TInterceptor> : Interceptor
        where TInterceptor : IInterceptor
    {
        /// <summary>
        /// Initialize an instance of <see cref="Interceptor{TInterceptor}"/> that will
        /// resolve the given interceptor type.
        /// </summary>
        public Interceptor()
            : base(typeof(TInterceptor))
        {
        }

        /// <summary>
        /// Initialize an instance of <see cref="Interceptor{TInterceptor}"/> that will
        /// resolve the given interceptor type and name.
        /// </summary>
        /// <param name="name">Name that will be used to resolve the interceptor.</param>
        public Interceptor(string name)
            : base(typeof(TInterceptor), name)
        {
        }
    }
}