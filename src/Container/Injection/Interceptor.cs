using System;
using Unity.Extension;
using Unity.Interception.ContainerIntegration;

namespace Unity.Interception
{
    /// <summary>
    /// Stores information about the <see cref="IInterceptor"/> to be used to intercept an object and
    /// configures a container accordingly.
    /// </summary>
    /// <seealso cref="IInterceptionBehavior"/>
    public class Interceptor : InterceptionMember
    {
        private IInterceptor? _interceptor;
        private readonly Type    _type;
        private readonly string? _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interceptor"/> class with an interceptor instance.
        /// </summary>
        /// <param name="interceptor">The <see cref="IInterceptor"/> to use.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptor"/> is
        /// <see langword="null"/>.</exception>
        public Interceptor(IInterceptor interceptor)
        {
            _interceptor = interceptor;
            _type = interceptor.GetType();
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Interceptor"/> class with a given
        /// name and type that will be resolved to provide interception.
        /// </summary>
        /// <param name="interceptorType">Type of the interceptor</param>
        /// <param name="name">name to use to resolve.</param>
        public Interceptor(Type interceptorType, string? name)
        {
            // TODO: Validation
            //Guard.TypeIsAssignable(typeof(IInterceptor), interceptorType ?? 
            //      throw new ArgumentNullException(nameof(interceptorType)), 
            //                                      nameof(interceptorType));
            _type = interceptorType;
            _name = name;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Interceptor"/> class with
        /// a given type that will be resolved to provide interception.
        /// </summary>
        /// <param name="interceptorType">Type of the interceptor.</param>
        public Interceptor(Type interceptorType)
        {
            _type = interceptorType;
            _name = default;
        }

        /// <summary>
        /// Interceptor to use.
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        public IInterceptor GetInterceptor(Interception extension)
            => _interceptor ??= (IInterceptor)extension.Container!.Resolve(_type, _name)!;



        public bool IsInstanceInterceptor 
            => _interceptor is null
            ? typeof(IInstanceInterceptor).IsAssignableFrom(_type)
            : _interceptor is IInstanceInterceptor;

        public bool IsTypeInterceptor
            => _interceptor is null
            ? typeof(ITypeInterceptor).IsAssignableFrom(_type)
            : _interceptor is ITypeInterceptor;


        public override MatchRank Matches(Type other)
        {
            if (_type.Equals(other)) return MatchRank.ExactMatch;

            if (other.IsAssignableFrom(_type)) return MatchRank.Compatible;

            return MatchRank.NoMatch;
        }
    }

    /// <summary>
    /// Generic version of <see cref="Interceptor"/> that lets you specify an interceptor
    /// type using generic syntax.
    /// </summary>
    /// <typeparam name="TInterceptor">Type of interceptor</typeparam>
    public class Interceptor<TInterceptor> : Interceptor
        where TInterceptor : IInterceptor
    {
        #region Constructors

        /// <summary>
        /// Initialize an instance of <see cref="Interceptor{TInterceptor}"/> that will
        /// resolve the given interceptor type.
        /// </summary>
        public Interceptor()
            : base(typeof(TInterceptor))
        {
        }

        /// <summary>
        /// Initialize an instance of <see cref="Interceptor{TInterceptor}"/> that will
        /// resolve the given interceptor type and name.
        /// </summary>
        /// <param name="name">Name that will be used to resolve the interceptor.</param>
        public Interceptor(string name)
            : base(typeof(TInterceptor), name)
        {
        }

        #endregion
    }
}