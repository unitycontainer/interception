using Unity.Extension;

namespace Unity.Interception.Strategies
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that intercepts objects
    /// in the build chain by creating a proxy object.
    /// </summary>
    public partial class InstanceInterceptionStrategy : BuilderStrategy
    {
        #region Fields

        private readonly Schemes _interceptors;

        #endregion


        #region Constructors

        public InstanceInterceptionStrategy(Schemes interceptors)
        {
            _interceptors = interceptors;
        }

        #endregion

    }
}
