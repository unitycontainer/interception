// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Builder;
using Unity.Interception.Interceptors.InstanceInterceptors;

namespace Unity.Interception.ContainerIntegration.ObjectBuilder
{
    /// <summary>
    /// An implementation of <see cref="IInstanceInterceptionPolicy"/> that will
    /// resolve the interceptor through the container.
    /// </summary>
    public class ResolvedInstanceInterceptionPolicy : IInstanceInterceptionPolicy
    {
        private readonly NamedTypeBuildKey _buildKey;

        /// <summary>
        /// Construct a new <see cref="ResolvedInstanceInterceptionPolicy"/> that
        /// will resolve the interceptor using the given build key.
        /// </summary>
        /// <param name="buildKey">build key to resolve.</param>
        public ResolvedInstanceInterceptionPolicy(NamedTypeBuildKey buildKey)
        {
            this._buildKey = buildKey;
        }

        #region IInstanceInterceptionPolicy Members

        /// <summary>
        /// Interceptor to use.
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        public IInstanceInterceptor GetInterceptor(IBuilderContext context)
        {
            return (IInstanceInterceptor)(context ?? throw new ArgumentNullException(nameof(context)))
                .NewBuildUp(_buildKey.Type, _buildKey.Name);
        }

        #endregion
    }
}
