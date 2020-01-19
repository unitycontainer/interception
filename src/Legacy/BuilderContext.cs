using System;
using System.Diagnostics;
using System.Security;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Builder
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    [SecuritySafeCritical]
    [DebuggerDisplay("Resolving: {Type},  Name: {Name}")]
    public partial struct BuilderContext : IResolveContext
    {
        #region Fields

        public IPolicyList List;
        public ILifetimeContainer? Scope;
        public ResolverOverride[]? Overrides;

        #endregion


        #region IResolveContext

        public IUnityContainer Container => Lifetime?.Container!;

        public Type Type { get; set; }

        public string? Name { get; set; }

        public object? Resolve(Type type, string? name)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region IPolicyList

        public object? Get(Type policyInterface)
        {
            return List.Get(RegistrationType, Name, policyInterface) ??
                   Registration.Get(policyInterface);
        }

        public object? Get(Type? type, string? name, Type policyInterface)
        {
            throw new NotImplementedException();
        }

        public object? Get(Type type, Type policyInterface)
        {
            throw new NotImplementedException();
        }

        public void Set(Type policyInterface, object policy)
        {
            List.Set(RegistrationType, Name, policyInterface, policy);
        }

        public void Set(Type type, Type policyInterface, object policy)
        {
            throw new NotImplementedException();
        }

        public void Set(Type? type, string? name, Type policyInterface, object policy)
        {
            List.Set(type, name, policyInterface, policy);
        }

        public void Clear(Type? type, string? name, Type policyInterface)
        {
            List.Clear(type, name, policyInterface);
        }

        #endregion


        #region Registration

        public Type RegistrationType { get; set; }

        public IPolicySet Registration { get; set; }

        #endregion


        #region Public Properties

        public object? Existing { get; set; }

        public ILifetimeContainer Lifetime;

        public SynchronizedLifetimeManager? RequiresRecovery;

        public bool BuildComplete;

        public Type? DeclaringType;
#if !NET40
        public IntPtr Parent;
#endif

        #endregion


        /// <summary>
        /// Attempts to resolve an instance with requested <see cref="Type"/> and name.
        /// </summary>
        /// <param name="type">Type of the contract</param>
        /// <param name="name">Name of the contract</param>
        /// <returns>An instance of requested contract or <see cref="LifetimeManager.NoValue"/> value
        /// if requested object could not be created</returns>
        object TryResolve(Type type, string? name) 
        {
            return LifetimeManager.NoValue;
        }
    }
}
