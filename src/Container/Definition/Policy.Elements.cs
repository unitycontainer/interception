using System;
using Unity.Injection;
using Unity.Lifetime;

namespace Unity.Interception
{
    public partial class PolicyDefinition
    {
        private PolicyDefinition AddElement<T>(T instance)
        {
            _extension.Container!.RegisterInstance(Guid.NewGuid().ToString(), instance);

            return this;
        }

        private PolicyDefinition AddElement<T>(Type type, string name, LifetimeManager? lifetimeManager, InjectionMember[] injectionMembers)
        {
            _rules.Add(new ResolvedParameter<T>(name));
            _extension.Container!.RegisterType(typeof(T), type, name, (ITypeLifetimeManager?)lifetimeManager, injectionMembers);

            return this;
        }

        private PolicyDefinition AddElement<T, TElement>(string name, LifetimeManager? lifetimeManager, InjectionMember[] injectionMembers)
            where TElement : T => AddElement<T>(typeof(TElement), name, lifetimeManager, injectionMembers);
    }
}
