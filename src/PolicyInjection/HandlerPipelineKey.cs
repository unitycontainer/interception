

using System;
using System.Reflection;
using Unity.Interception.Utilities;

namespace Unity.Interception.PolicyInjection
{
    /// <summary>
    /// Key for handler pipelines.
    /// </summary>
    public struct HandlerPipelineKey : IEquatable<HandlerPipelineKey>
    {
        private readonly Module _module;
        private readonly int _methodMetadataToken;

        /// <summary>
        /// Creates a new <see cref="HandlerPipelineKey"/> for the supplied method.
        /// </summary>
        /// <param name="methodBase">The method for the key.</param>
        /// <returns>The new key.</returns>
        public static HandlerPipelineKey ForMethod(MethodBase methodBase)
        {
            Guard.ArgumentNotNull(methodBase, "methodBase");

            return new HandlerPipelineKey(methodBase.DeclaringType.Module, methodBase.MetadataToken);
        }

        private HandlerPipelineKey(Module module, int methodMetadataToken)
        {
            _module = module;
            _methodMetadataToken = methodMetadataToken;
        }

        /// <summary>
        /// Compare two <see cref="HandlerPipelineKey"/> instances.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>True if the two keys are equal, false if not.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is HandlerPipelineKey))
            {
                return false;
            }
            return this == (HandlerPipelineKey)obj;
        }

        /// <summary>
        /// Calculate a hash code for this instance.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return _module.GetHashCode() ^ _methodMetadataToken;
        }

        /// <summary>
        /// Compare two <see cref="HandlerPipelineKey"/> instances for equality.
        /// </summary>
        /// <param name="left">First of the two keys to compare.</param>
        /// <param name="right">Second of the two keys to compare.</param>
        /// <returns>True if the values of the keys are the same, else false.</returns>
        public static bool operator ==(HandlerPipelineKey left, HandlerPipelineKey right)
        {
            return left._module == right._module &&
                   left._methodMetadataToken == right._methodMetadataToken;
        }

        /// <summary>
        /// Compare two <see cref="HandlerPipelineKey"/> instances for inequality.
        /// </summary>
        /// <param name="left">First of the two keys to compare.</param>
        /// <param name="right">Second of the two keys to compare.</param>
        /// <returns>false if the values of the keys are the same, else true.</returns>
        public static bool operator !=(HandlerPipelineKey left, HandlerPipelineKey right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compare two <see cref="HandlerPipelineKey"/> instances.
        /// </summary>
        /// <param name="other">Object to compare to.</param>
        /// <returns>True if the two keys are equal, false if not.</returns>
        public bool Equals(HandlerPipelineKey other)
        {
            return this == other;
        }
    }
}
