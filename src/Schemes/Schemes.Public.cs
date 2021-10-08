using System;

namespace Unity.Interception
{
    public partial class Schemes
    {
        /// <summary>
        /// Get default policy for the type
        /// </summary>
        /// <param name="target">Type of the registration</param>
        /// <param name="type">Type of policy to retrieve</param>
        /// <returns>Instance of the policy or null if none found</returns>
        public ISequenceSegment? Get(Type type)
        {
            var meta = Meta;
            var hash = (uint)GetHashCode(type);
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (candidate.Target is null &&
                    ReferenceEquals(candidate.Name, Contract.AnyContractName) &&
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
        /// Get default policy for the type
        /// </summary>
        /// <param name="target">Type of the registration</param>
        /// <param name="type">Type of policy to retrieve</param>
        /// <returns>Instance of the policy or null if none found</returns>
        public ISequenceSegment? Get(Type? target, Type type)
        {
            var meta = Meta;
            var hash = (uint)GetHashCode(target, type);
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Target, target) &&
                    ReferenceEquals(candidate.Name, Contract.AnyContractName) &&
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
        /// <param name="target">Type of the registration</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="type">Type of policy to retrieve</param>
        /// <returns>Instance of the policy or null if none found</returns>
        public ISequenceSegment? Get(Type? target, string? name, Type type)
        {
            var meta = Meta;
            var hash = (uint)GetHashCode(target, name, type);
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Target, target) &&
                    ReferenceEquals(candidate.Type, type) &&
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
        public void Set(Type type, ISequenceSegment? value)
        {
            var hash = (uint)GetHashCode(type);

            lock (SyncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (candidate.Target is null &&
                        ReferenceEquals(candidate.Name, Contract.AnyContractName) &&
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
        /// Set default policy for the type
        /// </summary>
        /// <param name="target">Type of the registration</param>
        /// <param name="type">Type of policy to be set</param>
        /// <param name="value">Policy instance to be set</param>
        public void Set(Type? target, Type type, ISequenceSegment? value)
        {
            var hash = (uint)GetHashCode(target, type);

            lock (SyncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (ReferenceEquals(candidate.Target, target) &&
                        ReferenceEquals(candidate.Name, Contract.AnyContractName) &&
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
                Data[Count] = new Entry(hash, target, type, value);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
            }
        }

        /// <summary>
        /// Set policy
        /// </summary>
        /// <param name="target">Type of the registration</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="type">Type of policy to be set</param>
        /// <param name="value">Policy instance to be set</param>
        public void Set(Type? target, string? name, Type type, ISequenceSegment? value)
        {
            var hash = (uint)GetHashCode(target, name, type);

            lock (SyncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (ReferenceEquals(candidate.Target, target) &&
                        ReferenceEquals(candidate.Type, type) &&
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
                Data[Count] = new Entry(hash, target, name, type, value);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
            }
        }

        /// <summary>
        /// Remove specific policy from the list
        /// </summary>
        /// <param name="target">Type of the registration</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="type">Type of policy to be removed</param>
        public void Clear(Type type)
        {
            var meta = Meta;
            var hash = (uint)GetHashCode(type);
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (candidate.Target is null &&
                    ReferenceEquals(candidate.Name, Contract.AnyContractName) &&
                    ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    candidate.Value = null;
                    return;
                }

                position = meta[position].Location;
            }
        }

        /// <summary>
        /// Remove specific policy from the list
        /// </summary>
        /// <param name="target">Type of the registration</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="type">Type of policy to be removed</param>
        public void Clear(Type? target, Type type)
        {
            var meta = Meta;
            var hash = (uint)GetHashCode(target, type);
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Target, target) &&
                    ReferenceEquals(candidate.Name, Contract.AnyContractName) &&
                    ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    candidate.Value = null;
                    return;
                }

                position = meta[position].Location;
            }
        }

        /// <summary>
        /// Remove specific policy from the list
        /// </summary>
        /// <param name="target">Type of the registration</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="type">Type of policy to be removed</param>
        public void Clear(Type? target, string? name, Type type)
        {
            var meta = Meta;
            var hash = (uint)GetHashCode(target, name, type);
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Target, target) &&
                    ReferenceEquals(candidate.Type, type) &&
                    candidate.Name == name)
                {
                    // Found existing
                    candidate.Value = null;
                    return;
                }

                position = meta[position].Location;
            }
        }
    }
}
