// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Builder;
using Unity.Interception.Interceptors.TypeInterceptors;

namespace Unity.Interception.ContainerIntegration.ObjectBuilder
{
    /// <summary>
    /// An implementation of <see cref="ITypeInterceptionPolicy"/> that will
    /// resolve the interceptor through the container.
    /// </summary>
    public class ResolvedTypeInterceptionPolicy : ITypeInterceptionPolicy
    {
        private readonly Type _type;
        private readonly string _name;
        private ITypeInterceptor _policy;

        /// <summary>
        /// construct a new <see cref="ResolvedTypeInterceptionPolicy"/> that
        /// will resolve the interceptor with the given <paramref name="buildKey"/>.
        /// </summary>
        /// <param name="buildKey">The build key to use to resolve.</param>
        public ResolvedTypeInterceptionPolicy(NamedTypeBuildKey buildKey)
        {
            _type = buildKey.Type;
            _name = buildKey.Name;
        }

        /// <summary>
        /// Create a new <see cref="ResolvedTypeInterceptionPolicy"/> that
        /// will resolve the interceptor with the given <paramref name="buildKey"/>.
        /// </summary>
        /// <param name="type">Type of the policy</param>
        /// <param name="name">Name of the registration</param>
        public ResolvedTypeInterceptionPolicy(Type type, string name)
        {
            _type = type;
            _name = name;
        }

        #region ITypeInterceptionPolicy Members

        /// <summary>
        /// Interceptor to use to create type proxy
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        public ITypeInterceptor GetInterceptor(IUnityContainer container)
        {
            if (null == _policy)
                _policy = (ITypeInterceptor)container.Resolve(_type, _name, null);

            return _policy;
        }

        /// <summary>
        /// Cache for proxied type.
        /// </summary>
        public Type ProxyType { get; set; }

        #endregion
    }
}
