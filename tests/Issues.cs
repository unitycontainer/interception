using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.ContainerIntegration;
using Unity.Lifetime;
using System.Diagnostics;
using Unity;
using Unity.Interception;

namespace UnityInterception.Tests
{
    [TestClass]
    public class Issues
    {

        [TestMethod]
        public void unitycontainer_unity_45()
        {
            var proxy = Intercept.ThroughProxy<IInterface<string, string>>(
              new Thing(),
              new InterfaceInterceptor(),
              new[] { new TestInterceptor() });

            proxy.DoSomething("hello world");
        }

        public interface IInterface<in TIn, out TOut>
        {
            TOut DoSomething(TIn input);
        }

        public class Thing : IInterface<string, string>
        {
            public string DoSomething(string input)
            {
                return input;
            }
        }

        public class TestInterceptor : IInterceptionBehavior
        {
            public bool WillExecute => true;

            public IEnumerable<Type> GetRequiredInterfaces()
            {
                return Type.EmptyTypes;
            }

            public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
            {
                return getNext()(input, null);
            }
        }


        [TestMethod]
        public void unitycontainer_interception_162()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.RegisterType<IMyClass, MyClass>(new ContainerControlledLifetimeManager(),
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>());

            var class1 = container.Resolve<IMyClass>();
            var class2 = container.Resolve<IMyClass>();
            class1.MyFunction();
            class2.MyFunction();
        }

        public interface IMyClass { void MyFunction(); }
        public class MyClass : IMyClass
        {
            public MyClass() { Debug.WriteLine("Create MyClass"); }
            public void MyFunction() { Debug.WriteLine("Function of MyClass"); }
        }
        public class LoggingInterceptionBehavior : IInterceptionBehavior
        {
            public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
            {
                Debug.WriteLine("Logging before the Function!! yeah, i like it!");
                return getNext()(input, getNext);
            }

            public bool WillExecute => true;
            public IEnumerable<Type> GetRequiredInterfaces() { return Type.EmptyTypes; }
        }

    }
}
