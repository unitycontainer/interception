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

        internal static AdditionalInterfacesPolicy GetOrCreate<TPolicySet>(ref TPolicySet policies)
            where  TPolicySet : IPolicySet
        {
            var policy = policies.Get<IAdditionalInterfacesPolicy>();
            if (!(policy is AdditionalInterfacesPolicy))
            {
                policy = new AdditionalInterfacesPolicy();
                policies.Set<IAdditionalInterfacesPolicy>(policy);
            }
            return (AdditionalInterfacesPolicy)policy;
        }
    }
}
