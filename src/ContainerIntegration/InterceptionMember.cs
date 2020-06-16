using System;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Interception.ContainerIntegration
{
    /// <summary>
    /// Stores information about a an intercepted object and configures a container accordingly.
    /// </summary>
    public abstract class InterceptionMember : InjectionMember, IAddPolicies
    {
        public virtual void AddPolicies<TPolicySet>(Type type, string name, ref TPolicySet policies)
            where TPolicySet : IPolicySet
        { 
        }
    }
}
