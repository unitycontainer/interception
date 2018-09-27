

using System;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Interception.ContainerIntegration.ObjectBuilder
{
    /// <summary>
    /// An <see cref="IBuilderPolicy"/> that returns a sequence of <see cref="Type"/> 
    /// instances representing the additional interfaces for an intercepted object.
    /// </summary>
    public interface IAdditionalInterfacesPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Gets the <see cref="Type"/> instances accumulated by this policy.
        /// </summary>
        IEnumerable<Type> AdditionalInterfaces { get; }
    }
}
