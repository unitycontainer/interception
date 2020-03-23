using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Unit.Tests;
using Unity;
using Unity.Interception;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Interception.PolicyInjection.MatchingRules;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.Tests;
using Unity.Lifetime;

namespace Extension.Tests
{
    [TestClass]
    public class InterfaceInterception : TestFixtureBase
    {
        [TestMethod]
        public void CanInterceptInstancesViaTheContainer()
        {
            Container
                .RegisterType<IDal, MockDal>()
                .Configure<Interception>()
                    .SetInterceptorFor<IDal>(new InterfaceInterceptor())
                    .AddPolicy("AlwaysMatches")
                    .AddMatchingRule<AlwaysMatchingRule>()
                    .AddCallHandler<CallCountHandler>("callCount", new ContainerControlledLifetimeManager());

            IDal dal = Container.Resolve<IDal>();

            Assert.IsTrue(dal is IInterceptingProxy);

            dal.Deposit(50.0);
            dal.Deposit(65.0);
            dal.Withdraw(15.0);

            CallCountHandler handler = (CallCountHandler)(Container.Resolve<ICallHandler>("callCount"));
            Assert.AreEqual(3, handler.CallCount);
        }

        [TestMethod]
        public void CanInterceptOpenGenericInterfaces()
        {
            Container
                .RegisterType(typeof(InterfaceInterceptorFixture.IGenericOne<>), typeof(InterfaceInterceptorFixture.GenericImplementationOne<>))
                .Configure<Interception>()
                    .SetInterceptorFor(typeof(InterfaceInterceptorFixture.IGenericOne<>), new InterfaceInterceptor())
                    .AddPolicy("AlwaysMatches")
                    .AddMatchingRule<AlwaysMatchingRule>()
                    .AddCallHandler<CallCountHandler>("callCount", new ContainerControlledLifetimeManager());

            InterfaceInterceptorFixture.IGenericOne<decimal> target = Container.Resolve<InterfaceInterceptorFixture.IGenericOne<decimal>>();

            decimal result = target.DoSomething(52m);
            Assert.AreEqual(52m, result);
            target.DoSomething(17m);
            target.DoSomething(84.2m);

            CallCountHandler handler = (CallCountHandler)(Container.Resolve<ICallHandler>("callCount"));
            Assert.AreEqual(3, handler.CallCount);
        }

        [TestMethod]
        public void CanInterceptGenericInterfaceWithConstraints()
        {
            Container
                .RegisterType(typeof(IGenericInterfaceWithConstraints<>), typeof(ImplementsGenericInterface<>),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior(new NoopBehavior()));

            var result = Container.Resolve<IGenericInterfaceWithConstraints<MockDal>>();

            Assert.IsNotNull(result as IInterceptingProxy);
        }
    }


    #region Test Data

    public interface IGenericInterfaceWithConstraints<T>
        where T : class
    {
        void TestMethod1<T1>();

        void TestMethod2<T2>()
            where T2 : struct;

        void TestMethod3<T3>()
            where T3 : class;

        void TestMethod4<T4>()
            where T4 : class, new();

        void TestMethod5<T5>()
            where T5 : InjectionPolicy;

        void TestMethod6<T6>()
            where T6 : IMatchingRule;
    }

    public class ImplementsGenericInterface<T> : IGenericInterfaceWithConstraints<T>
        where T : class
    {
        public void TestMethod1<T1>()
        {
            throw new NotImplementedException();
        }

        public void TestMethod2<T2>() where T2 : struct
        {
            throw new NotImplementedException();
        }

        public void TestMethod3<T3>() where T3 : class
        {
            throw new NotImplementedException();
        }

        public void TestMethod4<T4>() where T4 : class, new()
        {
            throw new NotImplementedException();
        }

        public void TestMethod5<T5>() where T5 : InjectionPolicy
        {
            throw new NotImplementedException();
        }

        public void TestMethod6<T6>() where T6 : IMatchingRule
        {
            throw new NotImplementedException();
        }
    }

    public class NoopBehavior : IInterceptionBehavior
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            return getNext()(input, getNext);
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Enumerable.Empty<Type>();
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }

    #endregion
}
