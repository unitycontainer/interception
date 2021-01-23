using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Storage;

namespace Unity.Interception
{
    public partial class Schemes : IEnumerable<ISequenceSegment?>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int GetHashCode(Type type)
            => Contract.AnyNameHash ^ type.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int GetHashCode(Type? target, Type type)
            => ((target?.GetHashCode() ?? 0) +  Contract.AnyNameHash) ^ type.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int GetHashCode(Type? target, string? name, Type type)
            => ((target?.GetHashCode() ?? 0) + (name?.GetHashCode() ??  37)) ^ type.GetHashCode();


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

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();



        #region Policy Storage

        [DebuggerDisplay("Policy = { Type?.Name }", Name = "{ Target?.Name }")]
        [CLSCompliant(false)]
        public struct Entry
        {
            #region Fields

            public readonly uint Hash;
            public readonly Type? Target;
            public readonly string? Name;
            public readonly Type  Type;
            public ISequenceSegment?  Value;

            #endregion


            #region Constructors

            public Entry(uint hash, Type type, ISequenceSegment? value)
            {
                Hash = hash;
                Target = null;
                Name = Contract.AnyContractName;
                Type = type;
                Value = value;
            }

            public Entry(uint hash, Type? target, Type type, ISequenceSegment? value)
            {
                Hash = hash;
                Target = target;
                Name = Contract.AnyContractName;
                Type = type;
                Value = value;
            }

            public Entry(uint hash, Type? target, string? name, Type type, ISequenceSegment? value)
            {
                Hash = hash;
                Target = target;
                Name = name;
                Type = type;
                Value = value;
            }

            #endregion
        }

        #endregion
    }
}
