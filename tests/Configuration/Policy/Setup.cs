using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity;
using Unity.Interception;
using Unity.Interception.Interceptors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Configuration
{
    [TestClass]
    public partial class PolicyFixture
    {
        #region Fields

        private const string TEST = "Testing";
        private const string EMPTY = "Empty";
        private const string RULES = "Rules";
        private const string MATCHING = "Matching";
        private const string HANDLERS = "Handlers";
        private const string Name = "name";
        private const string PolicyName = "Policy1";

        IMatchingRule rule = new AlwaysMatchingRule();
        ICallHandler handler = new InvokeCountHandler();
        MethodImplementationInfo info = new MethodImplementationInfo(null, typeof(NewsService).GetMethod(nameof(NewsService.GetNews)));

        public IUnityContainer Container { get; private set; }

        #endregion


        #region Scaffolding

        [TestInitialize]
        public void TestInitialize() => Container = new UnityContainer()
            .AddNewExtension<Interception>()
            .RegisterType<ICallHandler, Handler1>("handler1")
            .RegisterType<ICallHandler, Handler2>("handler2")
            .RegisterType<ICallHandler, Handler3>("handler3");

        #endregion

        private static MethodImplementationInfo GetMethodImplInfo<T>(string methodName)
            => new MethodImplementationInfo(null, typeof(T).GetMethod(methodName));
    }


    #region Test Data

    public class Handler1 : ICallHandler
    {
        private int order = 0;

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public IMethodReturn Invoke(IMethodInvocation input,
                                    GetNextHandlerDelegate getNext)
        {
            throw new NotImplementedException();
        }
    }

    public class Handler2 : ICallHandler
    {
        private int order = 0;

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public IMethodReturn Invoke(IMethodInvocation input,
                                    GetNextHandlerDelegate getNext)
        {
            throw new NotImplementedException();
        }
    }

    public class Handler3 : ICallHandler
    {
        private int order = 0;

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public IMethodReturn Invoke(IMethodInvocation input,
                                    GetNextHandlerDelegate getNext)
        {
            throw new NotImplementedException();
        }
    }

    public interface IYetAnotherInterface
    {
        void MyMethod();
    }

    public class YetAnotherMyType : IYetAnotherInterface
    {
        public void MyMethod() { }
    }

    public class TypeComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x.GetType() == y.GetType())
            {
                return 0;
            }
            return -1;
        }
    }

    public class AlwaysMatchingRule : IMatchingRule
    {
        public bool Matches(MethodBase member) => true;
    }

    public interface INewsService
    {
        IList GetNews();
    }

    public class NewsService : INewsService
    {
        public IList GetNews()
        {
            return new List<string> { "News1", "News2", "News3" };
        }
    }

    #endregion
}
