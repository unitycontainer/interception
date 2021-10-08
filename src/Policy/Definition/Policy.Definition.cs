using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unity.Interception
{
    /// <summary>
    /// Transient class that supports convenience method for specifying interception policies.
    /// </summary>
    [DebuggerDisplay("Policy: {Name}")]
    public partial class PolicyDefinition
    {
        #region Fields

        private readonly string       _name;
        private readonly Interception _extension;

        private readonly IList<object> _rules;
        private readonly IList<object> _handlers;

        #endregion


        #region Constructors

        internal PolicyDefinition(string? policyName, Interception extension)
        {
            _name = policyName ?? Guid.NewGuid().ToString(); 
            _extension = extension;

            _rules    = new List<object>();
            _handlers = new List<object>();
        }

        #endregion


        public string Name => _name;

        /// <summary>
        /// The <see cref="Interception"/> extension to which the policy was added.
        /// </summary>
        /// <remarks>
        /// Use this property to start adding a new policy.
        /// </remarks>
        public Interception Interception => _extension;
    }
}
