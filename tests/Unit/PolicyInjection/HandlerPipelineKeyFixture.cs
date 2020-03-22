using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Unity.Interception.PolicyInjection;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.PolicyInjection
{
    [TestClass]
    public class HandlerPipelineKeyFixture
    {
        private static readonly MethodInfo Method1 = typeof(Base).GetMethod(nameof(Base.Method1));
        private static readonly MethodInfo Method2 = typeof(Base).GetMethod(nameof(Base.Method2));
        private static readonly MethodInfo ToStringMethod = typeof(Base).GetMethod(nameof(Base.ToString));

        [TestMethod]
        public void KeysForSameMethodAreEqual()
        {
            var key1 = HandlerPipelineKey.ForMethod(Method1);
            var key2 = HandlerPipelineKey.ForMethod(Method1);

            Assert.AreEqual(key1, key2);
        }

        [TestMethod]
        public void KeysForSameMethodReflectedFromDifferentTypesAreEqual()
        {
            var key1 = HandlerPipelineKey.ForMethod(Method2);
            var key2 = HandlerPipelineKey.ForMethod(Method2);

            Assert.AreEqual(key1, key2);
        }

        [TestMethod]
        public void KeysForSameMethodReflectedFromDifferentTypesOnDifferentModulesAreEqual()
        {
            var key1 = HandlerPipelineKey.ForMethod(ToStringMethod);
            var key2 = HandlerPipelineKey.ForMethod(ToStringMethod);

            Assert.AreEqual(key1, key2);
        }

        [TestMethod]
        public void KeysForDifferentMethodsAreNotEqual()
        {
            var key1 = HandlerPipelineKey.ForMethod(Method1);
            var key2 = HandlerPipelineKey.ForMethod(Method2);

            Assert.AreNotEqual(key1, key2);
        }

        [TestMethod]
        public void KeysForOverridenMethodReflectedFromDifferentTypesAreNotEqual()
        {
            // using plain reflection - lambdas get optimized so we cannot get the override through them
            var key1 = HandlerPipelineKey.ForMethod(Method1);
            var key2 = HandlerPipelineKey.ForMethod(typeof(Derived).GetMethod(nameof(Derived.Method1)));

            Assert.AreNotEqual(key1, key2);
        }

        public class Base
        {
            public virtual void Method1() { }
            public virtual void Method2() { }
            public override string ToString()
            {
                return base.ToString();
            }
        }

        public class Derived : Base
        {
            public override void Method1()
            {
                base.Method1();
            }
        }
    }
}
