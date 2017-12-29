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
        private readonly Type _type;
        private readonly string _name;

        /// <summary>
        /// Construct a new <see cref="ResolvedInstanceInterceptionPolicy"/> that
        /// will resolve the interceptor using the given build key.
        /// </summary>
        /// <param name="buildKey">build key to resolve.</param>
        public ResolvedInstanceInterceptionPolicy(NamedTypeBuildKey buildKey)
        {
            _type = buildKey.Type;
            _name = buildKey.Name;
        }

        /// <summary>
        /// Construct a new <see cref="ResolvedInstanceInterceptionPolicy"/> that
        /// will resolve the interceptor using the given build key.
        /// </summary>
        /// <param name="type">Type of interceptor</param>
        /// <param name="name">Name of registration</param>
        public ResolvedInstanceInterceptionPolicy(Type type, string name)
        {
            _type = type;
            _name = name;
        }

        #region IInstanceInterceptionPolicy

        /// <summary>
        /// Interceptor to use.
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        public IInstanceInterceptor GetInterceptor(IBuilderContext context)
        {
            return (IInstanceInterceptor)(context ?? throw new ArgumentNullException(nameof(context)))
                .NewBuildUp(_type, _name);
        }

        #endregion
    }
}
