using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity.Interception.Tests;

namespace Storage
{
    public partial class Schemes
    {
        #region Set

        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, SET)]
        [ExpectedException(typeof(NullReferenceException))]
        public void Set_null_instance()
        {
            // Act
            SchemeStorage.Set(null, Instance);
        }

        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, SET)]
        public void Set_type_instance()
        {
            // Act
            SchemeStorage.Set(typeof(object), Instance);

            // Validate
            var entry = SchemeStorage.ToArray();
            
            Assert.AreEqual(1, entry.Length);
            Assert.AreSame(Instance, entry[0]);
            Assert.IsNull(Instance.Next);
        }

        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, SET)]
        public void Set_type_replace()
        {
            // Act
            SchemeStorage.Set(typeof(object), Instance);
            SchemeStorage.Set(typeof(object), Other);

            // Validate
            var entry = SchemeStorage.ToArray();

            Assert.AreEqual(1, entry.Length);
            Assert.AreSame(Other, entry[0]);
            Assert.IsNotNull(Other.Next);
            Assert.AreSame(Other.Next, Instance);
            Assert.IsNull(Instance.Next);
        }


        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, SET)]
        public void Set_type_set2list()
        {
            // Act
            SchemeStorage.Set(typeof(object), Instance);
            SchemeStorage.Set(null, typeof(object), Other);

            // Validate
            var entry = SchemeStorage.ToArray();

            Assert.AreEqual(1, entry.Length);
            Assert.AreSame(Other, entry[0]);
            Assert.IsNotNull(Other.Next);
            Assert.AreSame(Other.Next, Instance);
            Assert.IsNull(Instance.Next);
        }


        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, SET)]
        public void Set_different_instance()
        {
            // Act
            SchemeStorage.Set(typeof(Type), Instance);
            SchemeStorage.Set(typeof(object), Instance);

            // Validate
            var entry = SchemeStorage.ToArray();

            Assert.AreEqual(2, entry.Length);
            Assert.AreSame(Instance, entry[0]);
            Assert.AreSame(Instance, entry[1]);
            Assert.IsNull(Instance.Next);
        }


        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, SET)]
        public void Set_overload_instance()
        {
            // Act
            foreach (var type in TestTypes)
            {
                SchemeStorage.Set(type, Instance);
            }

            // Validate
            var entry = SchemeStorage.ToArray();

            Assert.AreEqual(TestTypes.Length, entry.Length);
            Assert.IsNull(Instance.Next);
        }


        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, SET)]
        public void Set_overload_twice()
        {
            // Act
            foreach (var type in TestTypes)
            {
                SchemeStorage.Set(type, Instance);
            }

            foreach (var type in TestTypes)
            {
                SchemeStorage.Set(type, Instance);
            }

            // Validate
            var entry = SchemeStorage.ToArray();
            Assert.AreEqual(TestTypes.Length, entry.Length);
            Assert.IsNotNull(Instance.Next);
        }

        #endregion


        #region Get

        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, SET)]
        public void Get_type_instance()
        {
            // Act
            foreach (var type in TestTypes)
            {
                SchemeStorage.Set(type, Instance);
            }

            // Validate
            foreach (var type in TestTypes)
            {

                var value = SchemeStorage.Get(type);
                Assert.AreSame(Instance, value);
            }
        }


        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, SET)]
        public void Get_type_miss()
        {
            // Arrange
            foreach (var type in TestTypes)
                SchemeStorage.Set(typeof(object), type, Instance);

            foreach (var type in TestTypes.Take(TestTypes.Length - 10))
                SchemeStorage.Set(type, Instance);

            // Validate
            Assert.IsNull(SchemeStorage.Get(TestTypes[TestTypes.Length - 1]));
            Assert.IsNull(SchemeStorage.Get(TestTypes[TestTypes.Length - 2]));
            Assert.IsNull(SchemeStorage.Get(TestTypes[TestTypes.Length - 3]));
            Assert.IsNull(SchemeStorage.Get(TestTypes[TestTypes.Length - 4]));
            Assert.IsNull(SchemeStorage.Get(TestTypes[TestTypes.Length - 5]));
        }

        #endregion


        #region Clear

        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, SET)]
        public void Clear_type_hit()
        {
            foreach (var type in TestTypes) SchemeStorage.Set(type, Instance);
            foreach (var type in TestTypes) SchemeStorage.Clear(type);
            foreach (var type in TestTypes) Assert.IsNull(SchemeStorage.Get(type));
        }


        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, SET)]
        public void Clear_type_miss()
        {
            // Act
            SchemeStorage.Set(typeof(object), Instance);
            SchemeStorage.Clear(typeof(Type));
            Assert.IsNotNull(SchemeStorage.Get(typeof(object)));
        }

        #endregion
    }
}
