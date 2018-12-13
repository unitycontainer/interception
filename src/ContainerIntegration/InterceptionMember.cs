using System;
using Unity.Injection;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Interception.ContainerIntegration
{
    /// <summary>
    /// Stores information about a an intercepted object and configures a container accordingly.
    /// </summary>
    public abstract class InterceptionMember : IInjectionMember
    {
        public virtual bool BuildRequired => false;

        public virtual void AddPolicies<TContext, TPolicyList>(Type registeredType, Type mappedToType, string name,
            ref TPolicyList policies)
            where TContext : IResolveContext
            where TPolicyList : IPolicyList
        {
        }
    }
}
