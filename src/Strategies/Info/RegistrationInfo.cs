using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Extension;

namespace Unity.Interception.Strategies
{
    public struct RegistrationInfo<TContext>
        where TContext : IBuilderContext
    {
        #region Fields
        
        private readonly ISequenceSegment? _registered;
        private readonly ISequenceSegment? _namedType;
        private readonly ISequenceSegment? _namedGeneric;
        private readonly ISequenceSegment? _type;
        private readonly ISequenceSegment? _generic;

        #endregion


        public RegistrationInfo(ref TContext context, Interception extension)
        {
            _registered = context.Registration?.Policies;
            _namedType = extension.Get(context.Contract.Type, context.Contract.Name);
            _type = extension.Get(context.Contract.Type);

            if (context.TypeDefinition is null)
            {
                _namedGeneric = default;
                _generic = default;
            }
            else
            {
                _namedGeneric = extension.Get(context.TypeDefinition, context.Contract.Name);
                _generic = extension.Get(context.TypeDefinition); 
            }
        }
    }
}
