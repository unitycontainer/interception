

using Unity.Builder;
using Unity.Builder.Selection;
using Unity.Policy;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception
{
    /// <summary>
    /// A small implementation of <see cref="IConstructorSelectorPolicy"/> that returns the
    /// given <see cref="SelectedConstructor"/> object.
    /// </summary>
    public class ConstructorWithResolverKeysSelectorPolicy : IConstructorSelectorPolicy
    {
        private readonly SelectedConstructor _selectedConstructor;

        /// <summary>
        /// Create a new <see cref="ConstructorWithResolverKeysSelectorPolicy"/> instance.
        /// </summary>
        /// <param name="selectedConstructor">Information about which constructor to select.</param>
        public ConstructorWithResolverKeysSelectorPolicy(SelectedConstructor selectedConstructor)
        {
            _selectedConstructor = selectedConstructor;
        }

        /// <summary>
        /// Choose the constructor to call for the given type.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>The chosen constructor.</returns>
        public SelectedConstructor SelectConstructor(IBuilderContext context)
        {
            return _selectedConstructor;
        }
    }
}
