// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Interception.ContainerIntegration.ObjectBuilder
{
    /// <summary>
    /// An <see cref="IAdditionalInterfacesPolicy"/> that accumulates a sequence of 
    /// <see cref="Type"/> instances representing the additional interfaces for an intercepted object.
    /// </summary>
    public class AdditionalInterfacesPolicy : IAdditionalInterfacesPolicy
    {
        private readonly List<Type> _additionalInterfaces;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionalInterfacesPolicy"/> class.
        /// </summary>
        public AdditionalInterfacesPolicy()
        {
            _additionalInterfaces = new List<Type>();
        }

        /// <summary>
        /// Gets the <see cref="Type"/> instances accumulated by this policy.
        /// </summary>
        public IEnumerable<Type> AdditionalInterfaces => _additionalInterfaces;

        internal void AddAdditionalInterface(Type additionalInterface)
        {
            _additionalInterfaces.Add(additionalInterface);
        }

        internal static AdditionalInterfacesPolicy GetOrCreate(IPolicyList policies,
                                                               Type typeToCreate,
                                                               string name)
        {
            NamedTypeBuildKey key = new NamedTypeBuildKey(typeToCreate, name);
            IAdditionalInterfacesPolicy policy =
                (IAdditionalInterfacesPolicy)policies.Get(typeToCreate, name, typeof(IAdditionalInterfacesPolicy), out _);
            if (!(policy is AdditionalInterfacesPolicy))
            {
                policy = new AdditionalInterfacesPolicy();
                policies.Set(typeToCreate, name, typeof(IAdditionalInterfacesPolicy), policy);
            }
            return (AdditionalInterfacesPolicy)policy;
        }
    }
}
