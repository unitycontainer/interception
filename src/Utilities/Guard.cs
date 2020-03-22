using System;
using System.Globalization;
using System.Reflection;

namespace Unity.Interception.Utilities
{
    /// <summary>
    /// A static helper class that includes various parameter checking routines.
    /// </summary>
    internal static class Guard
    {
        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <param name="assignmentTargetType">The argument type that will be assigned to.</param>
        /// <param name="assignmentValueType">The type of the value being assigned.</param>
        /// <param name="argumentName">Argument name.</param>
        public static void TypeIsAssignable(Type assignmentTargetType, Type assignmentValueType, string argumentName)
        {
            if (assignmentTargetType == null)
            {
                throw new ArgumentNullException(nameof(assignmentTargetType));
            }

            if (assignmentValueType == null)
            {
                throw new ArgumentNullException(nameof(assignmentValueType));
            }

            if (!assignmentTargetType.GetTypeInfo().IsAssignableFrom(assignmentValueType.GetTypeInfo()))
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "The type {1} cannot be assigned to variables of type {0}.",
                    assignmentTargetType,
                    assignmentValueType),
                    argumentName);
            }
        }
    }
}
