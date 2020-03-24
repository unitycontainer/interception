using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Unity.Interception.Tests
{
    public class NaiveINotifyPropertyChangedInterceptionBehavior : IInterceptionBehavior
    {
        private readonly object _handlerLock = new object();
        private PropertyChangedEventHandler _handler;

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var methodReturn = InvokeINotifyPropertyChangedMethod(input);

            if (methodReturn != null)
            {
                return methodReturn;
            }

            methodReturn = getNext()(input, getNext);

            if (methodReturn.Exception == null && input.MethodBase.IsSpecialName)
            {
                var property = GetPropertyInfoForSetMethod(input);

                if (property != null)
                {
                    var currentHandler = _handler;
                    if (currentHandler != null)
                    {
                        try
                        {
                            currentHandler(input.Target, new PropertyChangedEventArgs(property.Name));
                        }
                        catch (Exception e)
                        {
                            return input.CreateExceptionMethodReturn(e);
                        }
                    }
                }
            }

            return methodReturn;
        }

        private IMethodReturn InvokeINotifyPropertyChangedMethod(IMethodInvocation input)
        {
            if (input.MethodBase.DeclaringType == typeof(INotifyPropertyChanged))
            {
                switch (input.MethodBase.Name)
                {
                    case "add_PropertyChanged":
                        lock (_handlerLock)
                        {
                            _handler = (PropertyChangedEventHandler)Delegate.Combine(_handler, (Delegate)input.Arguments[0]);
                        }
                        break;

                    case "remove_PropertyChanged":
                        lock (_handlerLock)
                        {
                            _handler = (PropertyChangedEventHandler)Delegate.Remove(_handler, (Delegate)input.Arguments[0]);
                        }
                        break;

                    default:
                        return input.CreateExceptionMethodReturn(new InvalidOperationException());
                }

                return input.CreateMethodReturn(null);
            }

            return null;
        }

        private static PropertyInfo GetPropertyInfoForSetMethod(IMethodInvocation input)
        {
            foreach (var property in input.MethodBase.DeclaringType.GetProperties())
            {
                if (input.MethodBase == property.GetSetMethod())
                {
                    return property;
                }
            }

            return null;
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return new[] { typeof(INotifyPropertyChanged) };
        }

        public bool WillExecute { get { return true; } }
    }
}
