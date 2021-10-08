using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Storage;

namespace Unity.Interception
{
    public partial class Interception
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int GetHashCode(Type? target, Type type)
            => ((target?.GetHashCode() ?? 0) + Contract.AnyNameHash) ^ type.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int GetHashCode(Type? target, string? name, Type type)
            => ((target?.GetHashCode() ?? 0) + (name?.GetHashCode() ?? 37)) ^ type.GetHashCode();


        protected virtual void Expand()
        {
            Array.Resize(ref Data, Storage.Prime.Numbers[Prime++]);
            Meta = new Metadata[Storage.Prime.Numbers[Prime]];

            for (var current = 1; current < Count; current++)
            {
                var bucket = Data[current].Hash % Meta.Length;
                Meta[current].Location = Meta[bucket].Position;
                Meta[bucket].Position = current;
            }
        }

        public IEnumerator<ISequenceSegment?> GetEnumerator()
        {
            for (var i = 1; i <= Count; i++)
            {
                yield return Data[i].Value;
            }
        }


        #region Policy Storage

        [DebuggerDisplay("Policy = { Type?.Name }", Name = "{ Target?.Name }")]
        [CLSCompliant(false)]
        public struct Entry
        {
            #region Fields

            public readonly uint Hash;
            public readonly Type? Type;
            public readonly string? Name;
            public ISequenceSegment? Value;

            #endregion


            #region Constructors

            public Entry(uint hash, ISequenceSegment? value)
            {
                Hash = hash;
                Type = null;
                Name = Contract.AnyContractName;
                Value = value;
            }

            public Entry(uint hash, Type? target, ISequenceSegment? value)
            {
                Hash = hash;
                Type = target;
                Name = Contract.AnyContractName;
                Value = value;
            }

            public Entry(uint hash, Type? target, string? name, ISequenceSegment? value)
            {
                Hash = hash;
                Type = target;
                Name = name;
                Value = value;
            }

            #endregion
        }

        #endregion
    }
}
