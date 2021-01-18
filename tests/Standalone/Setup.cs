using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Interception;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;
using Unity.Interception.TestSupport;

namespace Standalone
{
    [TestClass]
    public partial class StaticInterception
    {
        #region Constants

        const string PROPERTY = "Test";
        const string INSTANCE = "Instance";
        const string TYPE     = "Type";

        #endregion


        [TestMethod]
        public void CanInterceptNewInstanceWithTypeInterceptor()
        {
            bool invoked = false;
            IInterceptionBehavior interceptionBehavior =
                new TestDelegateBehavior((mi, next) => { invoked = true; return mi.CreateMethodReturn(100); });

            BaseClass instance = (BaseClass)
                Intercept.NewInstanceWithAdditionalInterfaces(
                    typeof(BaseClass),
                    new VirtualMethodInterceptor(),
                    new[] { interceptionBehavior },
                    Type.EmptyTypes,
                    10);

            int value = instance.DoSomething();

            Assert.AreEqual(100, value);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void CanInterceptNewInstanceWithTypeInterceptorUsingGenericVersion()
        {
            bool invoked = false;
            IInterceptionBehavior interceptionBehavior =
                new TestDelegateBehavior((mi, next) => { invoked = true; return mi.CreateMethodReturn(100); });

            BaseClass instance =
                Intercept.NewInstanceWithAdditionalInterfaces<BaseClass>(
                    new VirtualMethodInterceptor(),
                    new[] { interceptionBehavior },
                    Type.EmptyTypes,
                    10);

            int value = instance.DoSomething();

            Assert.AreEqual(100, value);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingNewInstanceOfANullTypeThrows()
        {
            Intercept.NewInstance(
                null,
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingNewInstanceWithANullInterceptorThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                null,
                new IInterceptionBehavior[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceOfTypeNotCompatibleWithTheInterceptorThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new RejectingInterceptor(),
                new IInterceptionBehavior[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingNewInstanceWithANullSetOfInterceptionBehaviorsThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingNewInstanceWithANullSetOfAdditionalInterfacesThrows()
        {
            Intercept.NewInstanceWithAdditionalInterfaces(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[0],
                null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfAdditionalInterfacesWithNullElementsThrows()
        {
            Intercept.NewInstanceWithAdditionalInterfaces(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[0],
                new Type[] { null });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfAdditionalInterfacesWithNonInterfaceElementsThrows()
        {
            Intercept.NewInstanceWithAdditionalInterfaces(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[0],
                new Type[] { typeof(object) });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfAdditionalInterfacesWithGenericInterfaceElementsThrows()
        {
            Intercept.NewInstanceWithAdditionalInterfaces(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[0],
                new Type[] { typeof(IComparable<>) });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfBehaviorsWithNullElementsThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[] { null });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfBehaviorsWithElementReturningNullRequiredInterfacesThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[] 
                {
                    new TestDelegateBehavior(null, () => null)
                });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfBehaviorsWithElementReturningRequiredInterfacesWithNullElementThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[] 
                {
                    new TestDelegateBehavior(null, () => new Type[] { null })
                });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfBehaviorsWithElementReturningRequiredInterfacesWithNonInterfaceElementThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[] 
                {
                    new TestDelegateBehavior(null, () => new Type[] { typeof(object) })
                });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfBehaviorsWithElementReturningRequiredInterfacesWithGenericInterfaceElementThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[] 
                {
                    new TestDelegateBehavior(null, () => new Type[] { typeof(IEnumerable<>) })
                });
        }

        [TestMethod]
        public void CanInterceptAbstractClassWithVirtualMethodInterceptor()
        {
            bool invoked = false;
            IInterceptionBehavior interceptionBehavior =
                new TestDelegateBehavior((mi, next) => { invoked = true; return mi.CreateMethodReturn(100); });

            AbstractClass instance =
                Intercept.NewInstance<AbstractClass>(
                    new VirtualMethodInterceptor(),
                    new[] { interceptionBehavior });

            int value = instance.DoSomething();

            Assert.AreEqual(100, value);
            Assert.IsTrue(invoked);
        }

        #region Test Data

        public class BaseClass : IInterface
        {
            private readonly int value;

            public BaseClass()
                : this(0)
            { }

            public BaseClass(int value)
            {
                this.value = value;
            }

            public virtual int DoSomething()
            {
                return value;
            }
        }

        public interface IInterface
        {
            int DoSomething();
        }

        public interface ISomeInterface
        {
            int DoSomethingElse();
        }

        public class RejectingInterceptor : IInstanceInterceptor, ITypeInterceptor
        {
            public bool CanIntercept(Type t)
            {
                return false;
            }

            public IEnumerable<MethodImplementationInfo> GetInterceptableMethods(Type interceptedType, Type implementationType)
            {
                throw new NotImplementedException();
            }

            public IInterceptingProxy CreateProxy(Type t, object target, params Type[] additionalInterfaces)
            {
                throw new NotImplementedException();
            }

            public Type CreateProxyType(Type t, params Type[] additionalInterfaces)
            {
                throw new NotImplementedException();
            }
        }

        public abstract class AbstractClass
        {
            public abstract int DoSomething();
        }

        #endregion
    }
}
