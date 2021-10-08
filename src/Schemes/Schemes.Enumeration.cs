using System;
using System.Collections.Generic;

namespace Unity.Interception
{
    public partial class Schemes
    {
        public IEnumerable<ISequenceSegment> OfType(Type type)
        {
            for (var current = Get(type);
                     current is not null; 
                     current = current.Next)
            {
                yield return current;
            }
        }

        public IEnumerable<ISequenceSegment> OfType(Type? target, Type type)
        {
            for (var current = Get(target, type);
                     current is not null;
                     current = current.Next)
            {
                yield return current;
            }
        }

        public IEnumerable<ISequenceSegment> OfType(Type? target, string? name, Type type)
        {
            for (var current = Get(target, name, type);
                     current is not null;
                     current = current.Next)
            {
                yield return current;
            }
        }
    }
}
