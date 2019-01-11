using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;

namespace Unity.Interception.ContainerIntegration.Selection
{
    /// <summary>
    /// Objects of this type encapsulate <see cref="ConstructorInfo"/> and resolve
    /// parameters.
    /// </summary>
    public class SelectedConstructor : MethodBase<ConstructorInfo>
    {
        /// <summary>
        /// Create a new <see cref="SelectedConstructor"/> instance which
        /// contains the given constructor.
        /// </summary>
        /// <param name="constructor">The constructor to wrap.</param>
        public SelectedConstructor(ConstructorInfo constructor)
            : base(constructor)
        {
        }

        public SelectedConstructor(ConstructorInfo info, object[] parameters)
            : base(info, parameters)
        {
        }

        /// <summary>
        /// The constructor this object wraps.
        /// </summary>
        public ConstructorInfo Constructor => Selection;


        #region Overrides

        public override IEnumerable<ConstructorInfo> DeclaredMembers(Type type)
        {
#if NETCOREAPP1_0 || NETSTANDARD1_0
            return type.GetTypeInfo().DeclaredConstructors
                       .Where(c => c.IsStatic == false && c.IsPublic);
#else
            return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
#endif
        }

        #endregion
    }
}
