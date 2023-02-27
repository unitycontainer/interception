﻿using System;
using System.Collections.Generic;

namespace Unity.Interception.Interceptors
{
    /// <summary>
    /// Base interface for type and instance based interceptor classes.
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// Can this interceptor generate a proxy for the given type?
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <returns>True if interception is possible, false if not.</returns>
        bool CanIntercept(Type t);

        /// <summary>
        /// Returns a sequence of methods on the given type that can be
        /// intercepted.
        /// </summary>
        /// <param name="interceptedType">Type that was specified when this interceptor
        /// was created (typically an interface).</param>
        /// <param name="implementationType">The concrete type of the implementing object.</param>
        /// <returns>Sequence of <see cref="MethodImplementationInfo"/> objects.</returns>
        IEnumerable<MethodImplementationInfo> GetInterceptableMethods(Type interceptedType, Type implementationType);
    }
}
