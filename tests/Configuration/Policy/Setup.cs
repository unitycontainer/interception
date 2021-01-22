using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity;
using Unity.Interception;
using Unity.Interception.Interceptors;
using Unity.Interception.PolicyInjection.MatchingRules;
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
        private const string HANDLERS = "Handlers";
        private const string Name = "name";
        private const string PolicyName = "Policy1";

        IMatchingRule rule = new AlwaysMatchingRule();
        ICallHandler handler = new CallCountHandler();

        MethodImplementationInfo method = new MethodImplementationInfo(null, typeof(NewsService).GetMethod(nameof(NewsService.GetNews)));

        private IUnityContainer Container;

        #endregion


        #region Scaffolding

        [TestInitialize]
        public void TestInitialize() => Container = new UnityContainer()
            .AddNewExtension<Interception>();

        #endregion
    }


    #region Test Data

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

    public class CallCountHandler : ICallHandler
    {
        private int callCount;
        private int order = 0;

        [InjectionConstructor]
        public CallCountHandler()
        {
        }

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order
        {
            get
            {
                return order;
            }
            set
            {
                order = value;
            }
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            ++callCount;
            return getNext()(input, getNext);
        }

        public int CallCount
        {
            get { return callCount; }
        }
    }

    #endregion
}
