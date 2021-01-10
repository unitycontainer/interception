using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Interception;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors;
using Unity.Interception.TestSupport;

namespace Standalone
{
    public partial class StaticInterception
    {

        [TestMethod("Can intercept with Instance interceptor"), TestProperty(PROPERTY, nameof(Intercept.ThroughProxyWithAdditionalInterfaces))]
        public void CanInterceptTargetWithInstanceInterceptor()
        {
            bool invoked = false;

            IInterceptionBehavior behavior =
                new TestDelegateBehavior((mi, next) => { invoked = true; return mi.CreateMethodReturn(100); });

            IInterface proxy = (IInterface)
                Intercept.ThroughProxyWithAdditionalInterfaces(
                    typeof(IInterface),
                    new BaseClass(10),
                    new InterfaceInterceptor(),
                    new[] { behavior },
                    Type.EmptyTypes);

            int value = proxy.DoSomething();

            Assert.AreEqual(100, value);
            Assert.IsTrue(invoked);
        }


        [TestMethod("Proxy Implements Provided Interfaces")]
        [TestProperty(PROPERTY, nameof(Intercept.ThroughProxyWithAdditionalInterfaces))]
        public void GeneratedProxyImplementsUserProvidedAdditionalInterfaces()
        {
            bool invoked = false;
            IInterceptionBehavior interceptionBehavior =
                new TestDelegateBehavior((mi, next) => { invoked = true; return mi.CreateMethodReturn(100); });

            IInterface proxy = (IInterface)
                Intercept.ThroughProxyWithAdditionalInterfaces(
                    typeof(IInterface),
                    new BaseClass(10),
                    new InterfaceInterceptor(),
                    new[] { interceptionBehavior },
                    new[] { typeof(ISomeInterface) });

            int value = ((ISomeInterface)proxy).DoSomethingElse();

            Assert.AreEqual(100, value);
            Assert.IsTrue(invoked);
        }


        [TestMethod("Proxy Implements Behavior and Interfaces")]
        [TestProperty(PROPERTY, nameof(Intercept.ThroughProxyWithAdditionalInterfaces))]
        public void GeneratedProxyImplementsInterceptionBehaviorProvidedAdditionalInterfaces()
        {
            bool invoked = false;
            IInterceptionBehavior interceptionBehavior =
                new TestDelegateBehavior(
                    (mi, next) => { invoked = true; return mi.CreateMethodReturn(100); },
                    () => new[] { typeof(ISomeInterface) });

            IInterface proxy = (IInterface)
                Intercept.ThroughProxyWithAdditionalInterfaces(
                    typeof(IInterface),
                    new BaseClass(10),
                    new InterfaceInterceptor(),
                    new[] { interceptionBehavior },
                    Type.EmptyTypes);

            int value = ((ISomeInterface)proxy).DoSomethingElse();

            Assert.AreEqual(100, value);
            Assert.IsTrue(invoked);
        }

        [TestMethod("Can Intercept Using Generic Version")]
        [TestProperty(PROPERTY, nameof(Intercept.ThroughProxyWithAdditionalInterfaces))]
        public void CanInterceptTargetWithInstanceInterceptorUsingGenericVersion()
        {
            bool invoked = false;
            IInterceptionBehavior interceptionBehavior =
                new TestDelegateBehavior((mi, next) => { invoked = true; return mi.CreateMethodReturn(100); });

            IInterface proxy =
                Intercept.ThroughProxyWithAdditionalInterfaces<IInterface>(
                    new BaseClass(10),
                    new InterfaceInterceptor(),
                    new[] { interceptionBehavior },
                    Type.EmptyTypes);

            int value = proxy.DoSomething();

            Assert.AreEqual(100, value);
            Assert.IsTrue(invoked);
        }

        [TestMethod("Not compatible Types Throw")]
        [TestProperty(PROPERTY, nameof(Intercept.ThroughProxyWithAdditionalInterfaces))]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingTypesNotCompatibleWithTheInterceptorThrows()
        {
            Intercept.ThroughProxyWithAdditionalInterfaces(
                typeof(IInterface),
                new BaseClass(),
                new RejectingInterceptor(),
                new IInterceptionBehavior[0],
                Type.EmptyTypes);
        }

        [TestMethod("Non interface type throws")]
        [TestProperty(PROPERTY, nameof(Intercept.ThroughProxyWithAdditionalInterfaces))]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingWithASetOfAdditionalInterfacesIncludingNonInterfaceTypeThrows()
        {
            Intercept.ThroughProxyWithAdditionalInterfaces(
                typeof(IInterface),
                new BaseClass(),
                new InterfaceInterceptor(),
                new IInterceptionBehavior[0],
                new[] { typeof(object) });
        }
    }
}
