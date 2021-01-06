using System.Reflection;
using Unity.Injection;

namespace Unity.Interception.ContainerIntegration.Selection
{
    /// <summary>
    /// Objects of this type encapsulate <see cref="ConstructorInfo"/> and resolve
    /// parameters.
    /// </summary>
    public class SelectedConstructor : InjectionMethodBase<ConstructorInfo>
    {
        public ConstructorInfo Info;

        /// <summary>
        /// Create a new <see cref="SelectedConstructor"/> instance which
        /// contains the given constructor.
        /// </summary>
        /// <param name="constructor">The constructor to wrap.</param>
        public SelectedConstructor(ConstructorInfo constructor)
            : base(".ctor")
        {
            Info = constructor;
        }

        public SelectedConstructor(ConstructorInfo info, object[] parameters)
            : base(".ctor", parameters)
        {
            Info = info;
        }
    }
}
