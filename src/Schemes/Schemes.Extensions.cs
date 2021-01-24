using System;
using Unity.Storage;

namespace Unity.Interception
{
    public static class SchemesExtensions
    {
        public static TPolicy? Get<TPolicy>(this Schemes schemes)
            where TPolicy : ISequenceSegment
            => (TPolicy)schemes.Get(typeof(TPolicy));

        public static TPolicy? Get<TPolicy>(this Schemes schemes, Type type)
            where TPolicy : ISequenceSegment
            => (TPolicy)schemes.Get(type, typeof(TPolicy));

        public static TPolicy? Get<TTarget, TPolicy>(this Schemes schemes)
            where TPolicy : ISequenceSegment
            => (TPolicy)schemes.Get(typeof(TTarget), typeof(TPolicy));


        public static TPolicy? Get<TPolicy>(this Schemes schemes, Type type, string? name)
            where TPolicy : ISequenceSegment
            => (TPolicy)schemes.Get(type, name, typeof(TPolicy));

        public static TPolicy? Get<TTarget, TPolicy>(this Schemes schemes, string? name)
            where TPolicy : ISequenceSegment
            => (TPolicy)schemes.Get(typeof(TTarget), name, typeof(TPolicy));



        public static void Set<TPolicy>(this Schemes schemes, TPolicy policy)
            where TPolicy : ISequenceSegment
            => schemes.Set(typeof(TPolicy), policy);

        public static void Set<TPolicy>(this Schemes schemes, Type type, TPolicy policy)
            where TPolicy : ISequenceSegment
            => schemes.Set(type, typeof(TPolicy), policy);

        public static void Set<TTarget, TPolicy>(this Schemes schemes, TPolicy policy)
            where TPolicy : ISequenceSegment
            => schemes.Set(typeof(TTarget), typeof(TPolicy), policy);


        public static void Set<TPolicy>(this Schemes schemes, Type type, string? name, TPolicy policy)
            where TPolicy : ISequenceSegment
            => schemes.Set(type, name, typeof(TPolicy), policy);

        public static void Set<TTarget, TPolicy>(this Schemes schemes, string? name, TPolicy policy)
            where TPolicy : ISequenceSegment
            => schemes.Set(typeof(TTarget), name, typeof(TPolicy), policy);



        public static void Clear<TPolicy>(this Schemes schemes)
            => schemes.Clear(typeof(TPolicy));

        public static void Clear<TPolicy>(this Schemes schemes, Type type)
            => schemes.Clear(type, typeof(TPolicy));

        public static void Clear<TTarget, TPolicy>(this Schemes schemes)
            => schemes.Clear(typeof(TTarget), typeof(TPolicy));


        public static void Clear<TPolicy>(this Schemes schemes, Type type, string? name)
            => schemes.Clear(type, name, typeof(TPolicy));

        public static void Clear<TTarget, TPolicy>(this Schemes schemes, string? name)
            => schemes.Clear(typeof(TTarget), name, typeof(TPolicy));
    }
}
