using System;
using System.Collections.Generic;
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

        private PolicyDefinition AddElement<T>(IList<object> list, Type type, LifetimeManager? lifetimeManager, InjectionMember[] injectionMembers)
        {
            var name = Guid.NewGuid().ToString();
            list.Add(new ResolvedParameter<T>(name));

            _extension.Container!.RegisterType(typeof(T), type, name, (ITypeLifetimeManager?)lifetimeManager, injectionMembers);
            return this;
        }

        private PolicyDefinition AddElement<T>(IList<object> list, Type type, string name, LifetimeManager? lifetimeManager, InjectionMember[] injectionMembers)
        {
            list.Add(new ResolvedParameter<T>(name));

            _extension.Container!.RegisterType(typeof(T), type, name, (ITypeLifetimeManager?)lifetimeManager, injectionMembers);
            return this;
        }
    }
}
