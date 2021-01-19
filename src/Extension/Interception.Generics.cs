using Unity.Interception.Interceptors;

namespace Unity.Interception
{
    public static class InterceptionConfiguratorExtensions
    {
        #region Set

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <typeparam name="T">Type to intercept</typeparam>
        /// <param name="name">Name type is registered under.</param>
        /// <param name="interceptor">Interceptor object to use.</param>
        /// <returns>This extension object.</returns>
        public static Interception SetInterceptorFor<T>(this Interception config, string name, ITypeInterceptor interceptor) 
            => config.SetInterceptorFor(typeof(T), name, interceptor);

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <typeparam name="T">Type to intercept</typeparam>
        /// <param name="interceptor">Interceptor object to use.</param>
        /// <returns>This extension object.</returns>
        public static Interception SetInterceptorFor<T>(this Interception config, ITypeInterceptor interceptor) 
            => config.SetInterceptorFor(typeof(T), null, interceptor);

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <typeparam name="T">Type to intercept.</typeparam>
        /// <param name="name">Name type is registered under.</param>
        /// <param name="interceptor">Instance interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public static Interception SetInterceptorFor<T>(this Interception config, string name, IInstanceInterceptor interceptor) 
            => config.SetInterceptorFor(typeof(T), name, interceptor);

        /// <summary>
        /// API to configure interception for a type.
        /// </summary>
        /// <typeparam name="T">Type to intercept.</typeparam>
        /// <param name="interceptor">Instance interceptor to use.</param>
        /// <returns>This extension object.</returns>
        public static Interception SetInterceptorFor<T>(this Interception config, IInstanceInterceptor interceptor) 
            => config.SetInterceptorFor(typeof(T), null, interceptor);

        #endregion


        #region Set Default

        /// <summary>
        /// Set the interceptor for a type, regardless of what name is used to resolve the instances.
        /// </summary>
        /// <typeparam name="TTypeToIntercept">Type to intercept</typeparam>
        /// <param name="interceptor">Interceptor instance.</param>
        /// <returns>This extension object.</returns>
        public static Interception SetDefaultInterceptorFor<TTypeToIntercept>(this Interception config, ITypeInterceptor interceptor) 
            => config.SetDefaultInterceptorFor(typeof(TTypeToIntercept), interceptor);

        /// <summary>
        /// API to configure the default interception settings for a type.
        /// </summary>
        /// <typeparam name="TTypeToIntercept">Type the interception is being configured for.</typeparam>
        /// <param name="interceptor">The interceptor to use by default.</param>
        /// <returns>This extension object.</returns>
        public static Interception SetDefaultInterceptorFor<TTypeToIntercept>(this Interception config, IInstanceInterceptor interceptor) 
            => config.SetDefaultInterceptorFor(typeof(TTypeToIntercept), interceptor);

        #endregion
    }
}
