using System;
using Unity.Injection;
using Unity.Lifetime;

namespace Unity.Interception
{
    public partial class PolicyDefinition
    {
        private PolicyDefinition AddElement<T>(string name, UpdateElements update)
        {
            return update(name);
        }

        private PolicyDefinition AddElement<T>(T instance, UpdateElements update)
        {
            string newName = Guid.NewGuid().ToString();
            Container.RegisterInstance(newName, instance);

            return update(newName);
        }

        private PolicyDefinition AddElement<T>(Type type, string name,
            LifetimeManager lifetimeManager, InjectionMember[] injectionMembers, UpdateElements update)
        {
            Container.RegisterType(typeof(T), type, name, (ITypeLifetimeManager)lifetimeManager, injectionMembers);

            return update(name);
        }

        private PolicyDefinition AddElement<T, TElement>(
            string name,
            LifetimeManager lifetimeManager,
            InjectionMember[] injectionMembers,
            UpdateElements update)
            where TElement : T
        {
            return AddElement<T>(typeof(TElement), name, lifetimeManager, injectionMembers, update);
        }
    }
}
