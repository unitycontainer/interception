using System;

namespace Unity.Interception
{
    public partial class Interception
    {
        /// <summary>
        /// Get default policy for the type
        /// </summary>
        /// <param name="target">Type of the registration</param>
        /// <param name="type">Type of policy to retrieve</param>
        /// <returns>Instance of the policy or null if none found</returns>
        internal ISequenceSegment? Get(Type type)
        {
            var meta = Meta;
            var hash = (uint)Contract.GetHashCode(type.GetHashCode(), Contract.AnyNameHash);
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Name, Contract.AnyContractName) &&
                    ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    return candidate.Value;
                }

                position = meta[position].Location;
            }

            return null;
        }

        /// <summary>
        /// Get policy
        /// </summary>
        /// <param name="type">Type of the registration</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="type">Type of policy to retrieve</param>
        /// <returns>Instance of the policy or null if none found</returns>
        internal ISequenceSegment? Get(Type type, string? name)
        {
            var meta = Meta;
            var hash = (uint)Contract.GetHashCode(type, name);
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Type, type) &&
                    candidate.Name == name)
                {
                    // Found existing
                    return candidate.Value;
                }

                position = meta[position].Location;
            }

            return null;
        }

        /// <summary>
        /// Set default policy for the type
        /// </summary>
        /// <param name="target">Type of the registration</param>
        /// <param name="type">Type of policy to be set</param>
        /// <param name="value">Policy instance to be set</param>
        internal void Set(Type type, ISequenceSegment? value)
        {
            var hash = (uint)Contract.GetHashCode(type.GetHashCode(), Contract.AnyNameHash);

            lock (SyncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (ReferenceEquals(candidate.Name, Contract.AnyContractName) &&
                        ReferenceEquals(candidate.Type, type))
                    {
                        // Found existing
                        if (value is not null) value.Next = candidate.Value;
                        candidate.Value = value;
                        return;
                    }

                    position = Meta[position].Location;
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new 
                Data[Count] = new Entry(hash, type, value);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
            }
        }

        /// <summary>
        /// Set policy
        /// </summary>
        /// <param name="type">Type of the registration</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="type">Type of policy to be set</param>
        /// <param name="value">Policy instance to be set</param>
        internal void Set(Type type, string? name, ISequenceSegment? value)
        {
            var hash = (uint)Contract.GetHashCode(type, name);

            lock (SyncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (ReferenceEquals(candidate.Type, type) &&
                        candidate.Name == name)
                    {
                        // Found existing
                        if (value is not null) value.Next = candidate.Value;
                        candidate.Value = value;
                        return;
                    }

                    position = Meta[position].Location;
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new 
                Data[Count] = new Entry(hash, type, name, value);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
            }
        }
    }
}
