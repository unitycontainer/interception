using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Security;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Utilities;

namespace Unity.Interception.Interceptors.InstanceInterceptors.TransparentProxyInterception
{
    /// <summary>
    /// Provides a mechanism for instantiating proxy objects and handling their method dispatch. 
    /// It is invoked by a call on the corresponding TransparentProxy
    /// object. It routes calls through the handlers as appropriate.
    /// 
    /// See also: https://devblogs.microsoft.com/dotnet/migrating-realproxy-usage-to-dispatchproxy
    /// </summary>
    public class InterceptingDispatchProxy : DispatchProxy, IInterceptingProxy
    {
        private readonly InterceptionBehaviorPipeline _interceptorsPipeline = new InterceptionBehaviorPipeline();
        private readonly ReadOnlyCollection<Type> _additionalInterfaces;

        /// <summary>
        /// Creates a new <see cref="InterceptingDispatchProxy"/> instance that applies
        /// the given policies to the given target object.
        /// </summary>
        /// <param name="target">Target object to intercept calls to.</param>
        /// <param name="classToProxy">Type to return as the type being proxied.</param>
        /// <param name="additionalInterfaces">Additional interfaces the proxy must implement.</param>
        public InterceptingDispatchProxy(object target, Type classToProxy, params Type[] interfaces)
        {
            Guard.ArgumentNotNull(target, "target");
            Target = target;
            _additionalInterfaces = new ReadOnlyCollection<Type>(interfaces);
            TypeName = target.GetType().FullName;
        }

        /// <summary>
        /// Returns the target of this intercepted call.
        /// </summary>
        /// <value>The target object.</value>
        public object Target { get; }

        #region IInterceptingProxy Members

        /// <summary>
        /// Adds a <see cref="IInterceptionBehavior"/> to the proxy.
        /// </summary>
        /// <param name="interceptor">The <see cref="IInterceptionBehavior"/> to add.</param>
        [SecuritySafeCritical]
        public void AddInterceptionBehavior(IInterceptionBehavior interceptor)
        {
            Guard.ArgumentNotNull(interceptor, "interceptor");

            _interceptorsPipeline.Add(interceptor);
        }

        #endregion

        #region IRemotingTypeInfo Members

        /// <summary>
        /// Checks whether the proxy that represents the specified object type can be cast to the type represented by the <see cref="T:System.Runtime.Remoting.IRemotingTypeInfo"></see> interface.
        /// </summary>
        /// <returns>
        /// true if cast will succeed; otherwise, false.
        /// </returns>
        /// <param name="fromType">The type to cast to. </param>
        /// <param name="o">The object for which to check casting. </param>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission. </exception>
        
        public bool CanCastTo(Type fromType, object o)
        {
            Guard.ArgumentNotNull(fromType, "fromType");
            Guard.ArgumentNotNull(o, "o");

            if (fromType == typeof(IInterceptingProxy))
            {
                return true;
            }

            if (fromType.IsAssignableFrom(o.GetType()))
            {
                return true;
            }

            foreach (Type @interface in _additionalInterfaces)
            {
                if (fromType.IsAssignableFrom(@interface))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets or sets the fully qualified type name of the server object in a <see cref="T:System.Runtime.Remoting.ObjRef"></see>.
        /// </summary>
        /// <value>
        /// The fully qualified type name of the server object in a <see cref="T:System.Runtime.Remoting.ObjRef"></see>.
        /// </value>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="Infrastructure" /></PermissionSet>
        public string TypeName
        {
            
            get;
            
            set;
        }

        #endregion

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            throw new NotImplementedException();
        }

        public IInterceptingProxy GetTransparentProxy()
        {
            return this;
        }
    }
}
