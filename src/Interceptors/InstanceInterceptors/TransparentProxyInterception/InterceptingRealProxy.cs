using System;
using Unity.Interception.InterceptionBehaviors;

namespace Unity.Interception.Interceptors.InstanceInterceptors.TransparentProxyInterception
{
    /// <summary>
    /// This class provides the remoting based interception mechanism. It is
    /// invoked by a call on the corresponding TransparentProxy
    /// object. It routes calls through the handlers as appropriate.
    /// </summary>
    [Obsolete("Due to discontinued support for Remoting in .NET 5.0 and up, Transparent interception is deprecated", true)]
    public class InterceptingRealProxy : IInterceptingProxy
    {
        public void AddInterceptionBehavior(IInterceptionBehavior interceptor)
        {
            throw new NotImplementedException();
        }
    }
}
