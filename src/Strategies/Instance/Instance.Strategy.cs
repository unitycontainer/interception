using Unity.Extension;

namespace Unity.Interception.Strategies
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that intercepts objects
    /// in the build chain by creating a proxy object.
    /// </summary>
    public partial class InstanceInterceptionStrategy : InterceptionStrategy
    {
        #region Constructors

        public InstanceInterceptionStrategy(Interception extension)
            : base(extension)
        { 
        }

        #endregion

    }
}
