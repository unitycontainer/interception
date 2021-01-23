using System;
using System.Diagnostics;
using Unity.Storage;

namespace Unity.Interception
{
    public partial class Schemes
    {
        #region Fields

        protected int Count;

        [CLSCompliant(false)]
        protected Entry[] Data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never), CLSCompliant(false)]
        protected Metadata[] Meta;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly object SyncRoot = new object();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected int Prime = 1;

        #endregion


        #region Constructors

        public Schemes()
        {
            Data = new Entry[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];
        }

        #endregion
    }
}
