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
        private readonly NamedTypeBuildKey _buildKey;

        /// <summary>
        /// construct a new <see cref="ResolvedTypeInterceptionPolicy"/> that
        /// will resolve the interceptor with the given <paramref name="buildKey"/>.
        /// </summary>
        /// <param name="buildKey">The build key to use to resolve.</param>
        public ResolvedTypeInterceptionPolicy(NamedTypeBuildKey buildKey)
        {
            _buildKey = buildKey;
        }

        #region ITypeInterceptionPolicy Members

        /// <summary>
        /// Interceptor to use to create type proxy
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        public ITypeInterceptor GetInterceptor(IBuilderContext context)
        {
            return (ITypeInterceptor)(context ?? throw new ArgumentNullException(nameof(context)))
                .NewBuildUp(_buildKey.Type, _buildKey.Name);
        }

        /// <summary>
        /// Cache for proxied type.
        /// </summary>
        public Type ProxyType { get; set; }

        #endregion
    }
}
