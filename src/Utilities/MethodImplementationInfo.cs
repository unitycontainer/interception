using System.Reflection;

namespace Unity.Interception
{
    /// <summary>
    /// A dumb data holder that returns the MethodInfo for both an
    /// interface method and the method that implements that interface
    /// method.
    /// </summary>
    public class MethodImplementationInfo
    {
        #region Constructors

        /// <summary>
        /// Construct a new <see cref="MethodImplementationInfo"/> which holds
        /// the given <see cref="MethodInfo"/> objects.
        /// </summary>
        /// <param name="interfaceMethodInfo">MethodInfo for the interface method (may be null if no interface).</param>
        /// <param name="implementationMethodInfo">MethodInfo for implementing method.</param>
        public MethodImplementationInfo(MethodInfo interfaceMethodInfo, MethodInfo implementationMethodInfo)
        {
            InterfaceMethodInfo = interfaceMethodInfo;
            ImplementationMethodInfo = implementationMethodInfo;
        }

        #endregion


        #region Properties

        /// <summary>
        /// The interface method MethodInfo.
        /// </summary>
        public MethodInfo InterfaceMethodInfo { get; }

        /// <summary>
        /// The implementing method MethodInfo.
        /// </summary>
        public MethodInfo ImplementationMethodInfo { get; }


        #endregion


        #region Implementation

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to the current <see cref="object" />.
        /// </summary>
        public override bool Equals(object? obj)
        {
            var other = obj as MethodImplementationInfo;
            
            return obj is not null && other is not null && this == other;
        }


        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        public override int GetHashCode()
        {
            int hash1 = InterfaceMethodInfo      != null ? InterfaceMethodInfo.GetHashCode()      : 0;
            int hash2 = ImplementationMethodInfo != null ? ImplementationMethodInfo.GetHashCode() : 0;
            return hash1 * 23 ^ hash2 * 7;
        }


        /// <summary>
        /// Standard equals operator
        /// </summary>
        public static bool operator ==(MethodImplementationInfo left, MethodImplementationInfo right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;

            return left.InterfaceMethodInfo      == right.InterfaceMethodInfo &&
                   left.ImplementationMethodInfo == right.ImplementationMethodInfo;
        }


        /// <summary>
        /// standard not equal operator.
        /// </summary>
        public static bool operator !=(MethodImplementationInfo left, MethodImplementationInfo right) 
            => !(left == right);


        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="object" />.
        /// </summary>
        public override string ToString() 
            => InterfaceMethodInfo == null
            ? $"No interface, implementation {ImplementationMethodInfo.DeclaringType!.Name}.{ImplementationMethodInfo.Name}"
            : $"Interface {InterfaceMethodInfo.DeclaringType!.Name}.{InterfaceMethodInfo.Name}, " +
              $"implementation {ImplementationMethodInfo.DeclaringType!.Name}.{ImplementationMethodInfo.Name}";

        #endregion
    }
}
