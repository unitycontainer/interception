using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Extension;

namespace Unit.Tests
{

    [TestClass]
    public partial class Schemes
    {
        #region Constants

        const string SET = "Get/Set/Clear(Type)";
        const string LIST = "Get/Set/Clear(Type, Type)";
        const string NAMED = "Get/Set/Clear(Type, String, Type)";
        const string ENUMERATIONS = "OfType(..)";
        const string INTERFACE    = "Testing";
        const string SET_PATTERN  = "{0}({1}, {2})";
        const string LIST_PATTERN = "{0}({1}, {2}, {3})";
        const string NAMED_PATTERN = "{0}({1}, {2}, {3}, {4})";
        const string SET_POLICIES  = "IPolicies.{0}({1}, {2})";
        const string LIST_POLICIES = "IPolicies.{0}({1}, {2}, {3})";
        const string EXCHANGE      = nameof(IPolicies);
        const string EXCHANGE_PATTERN = "{1}({2}).CompareExchange({3}, {4})";
        const string Name = "name";

        #endregion


        #region Fields

        TestSchemes SchemeStorage;
        TestInstance Instance;
        TestInstance Other;

        private static Type[] TestTypes;

        Type Target;
        Type Type;
        object Policy;

        #endregion


        #region Scaffolding

        [ClassInitialize]
        public static void InitializeClass(TestContext _)
        {
            TestTypes = Assembly.GetAssembly(typeof(int))
                                .DefinedTypes
                                .Where(t => t != typeof(IServiceProvider))
                                .Take(1500)
                                .ToArray();
        }


        [TestInitialize]
        public void InitializeTest()
        {
            SchemeStorage = new TestSchemes();
            Instance = new TestInstance();
            Other = new TestInstance();
        }

        #endregion


        #region Implementation

        private void OnPolicyChanged(Type target, Type type, object policy)
        {
            Target = target;
            Type = type;
            Policy = policy;
        }

        #endregion
    }

    #region Test Types

    public class TestSchemes : Unity.Interception.Schemes
    {
        public object SyncObject => SyncRoot;
    }

    public class TestInstance : ISequenceSegment
    {
        public ISequenceSegment Next { get; set; }
    }

    #endregion
}
