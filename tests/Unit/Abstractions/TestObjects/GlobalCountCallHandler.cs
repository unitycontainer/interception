using System.Collections.Generic;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;

namespace Unity.Interception.Tests
{
    public class GlobalCountCallHandler : ICallHandler
    {
        public static Dictionary<string, int> Calls = new Dictionary<string, int>();
        private string _callHandlerName;

        [InjectionConstructor]
        public GlobalCountCallHandler()
            : this("default")
        {
        }

        public GlobalCountCallHandler(string callHandlerName)
        {
            this._callHandlerName = callHandlerName;
        }

        #region ICallHandler Members

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order { get; set; } = 0;

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            if (!Calls.ContainsKey(_callHandlerName))
            {
                Calls.Add(_callHandlerName, 0);
            }
            Calls[_callHandlerName]++;

            return getNext().Invoke(input, getNext);
        }

        #endregion
    }

    public class GlobalCountCallHandlerAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer _)
        {
            return new GlobalCountCallHandler(HandlerName);
        }

        public string HandlerName { get; set; }
    }
}
