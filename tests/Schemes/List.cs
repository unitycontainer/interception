using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity.Interception.Tests;

namespace Unit.Tests
{
    public partial class Schemes
    {
        #region Set

        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        [ExpectedException(typeof(NullReferenceException))]
        public void Set_null_null_instance()
        {
            // Act
            SchemeStorage.Set(null, null, Instance);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        [ExpectedException(typeof(NullReferenceException))]
        public void Set_target_null_instance()
        {
            // Act
            SchemeStorage.Set(typeof(object), null, Instance);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        public void Set_null_type_instance()
        {
            // Act
            SchemeStorage.Set(null, typeof(object), Instance);

            // Validate
            var entry = SchemeStorage.ToArray();

            Assert.AreEqual(1, entry.Length);
            Assert.AreSame(Instance, entry[0]);
            Assert.IsNull(Instance.Next);
        }

        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        public void Set_target_type_replace()
        {

            // Act
            SchemeStorage.Set(typeof(object), typeof(object), Instance);
            SchemeStorage.Set(typeof(object), typeof(object), Other);

            // Validate
            var entry = SchemeStorage.ToArray();

            Assert.AreEqual(1, entry.Length);
            Assert.AreSame(Other, entry[0]);
            Assert.IsNotNull(Other.Next);
            Assert.AreSame(Other.Next, Instance);
            Assert.IsNull(Instance.Next);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        public void Set_target_type_list2set()
        {

            // Act
            SchemeStorage.Set(null, typeof(object), Instance);
            SchemeStorage.Set(typeof(object), Other);

            // Validate
            var entry = SchemeStorage.ToArray();

            Assert.AreEqual(1, entry.Length);
            Assert.AreSame(Other, entry[0]);
            Assert.IsNotNull(Other.Next);
            Assert.AreSame(Other.Next, Instance);
            Assert.IsNull(Instance.Next);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        public void Set_different_type_instance()
        {
            // Act
            SchemeStorage.Set(typeof(Type), typeof(object), Instance);
            SchemeStorage.Set(typeof(object), typeof(object), Instance);

            // Validate
            var entry = SchemeStorage.ToArray();

            Assert.AreEqual(2, entry.Length);
            Assert.AreSame(Instance, entry[0]);
            Assert.AreSame(Instance, entry[1]);
            Assert.IsNull(Instance.Next);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        public void Set_target_different_instance()
        {
            // Act
            SchemeStorage.Set(typeof(object), typeof(Type), Instance);
            SchemeStorage.Set(typeof(Type), typeof(object), Instance);

            // Validate
            var entry = SchemeStorage.ToArray();

            Assert.AreEqual(2, entry.Length);
            Assert.AreSame(Instance, entry[0]);
            Assert.AreSame(Instance, entry[1]);
            Assert.IsNull(Instance.Next);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        public void Set_overload_type_instance()
        {
            // Act
            foreach (var type in TestTypes)
            {
                SchemeStorage.Set(type, typeof(object), Instance);
            }

            // Validate
            var entry = SchemeStorage.ToArray();

            Assert.AreEqual(TestTypes.Length, entry.Length);
            Assert.IsNull(Instance.Next);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        public void Set_target_overload_instance()
        {
            // Act
            foreach (var type in TestTypes)
            {
                SchemeStorage.Set(typeof(object), type, Instance);
            }

            foreach (var type in TestTypes)
            {
                SchemeStorage.Set(typeof(string), type, Instance);
            }

            // Validate
            var entry = SchemeStorage.ToArray();
            Assert.AreEqual(TestTypes.Length * 2, entry.Length);
            Assert.IsNull(Instance.Next);
        }

        #endregion


        #region Get

        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        [ExpectedException(typeof(NullReferenceException))]
        public void Get_null_null_instance()
        {
            // Act
            _ = SchemeStorage.Get(null, (Type)null);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        [ExpectedException(typeof(NullReferenceException))]
        public void Get_target_null_instance()
        {
            // Act
            _ = SchemeStorage.Get(typeof(object), (Type)null);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        public void Get_null_type_instance()
        {
            // Act
            foreach (var type in TestTypes.Take(44))
            {
                SchemeStorage.Set(null, type, Instance);
            }

            // Validate
            foreach (var type in TestTypes.Take(44))
            {
                var value = SchemeStorage.Get(null, type);
                Assert.AreSame(Instance, value);
            }

            Assert.IsNull(SchemeStorage.Get(typeof(object), typeof(object)));
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        public void Get_same_type_instance()
        {

            // Act
            foreach (var type in TestTypes)
            {
                SchemeStorage.Set(typeof(object), type, Instance);
            }

            // Validate
            foreach (var type in TestTypes)
            {

                var value = SchemeStorage.Get(typeof(object), type);
                Assert.AreSame(Instance, value);
            }
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        public void Get_target_same_instance()
        {

            // Act
            foreach (var type in TestTypes)
            {
                SchemeStorage.Set(type, typeof(object), Instance);
            }

            // Validate
            foreach (var type in TestTypes)
            {

                var value = SchemeStorage.Get(type, typeof(object));
                Assert.AreSame(Instance, value);
            }
        }

        #endregion


        #region Clear

        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        public void Clear_null_type_hit()
        {
            foreach (var type in TestTypes) SchemeStorage.Set(null, type, Instance); 
            foreach (var type in TestTypes) SchemeStorage.Clear(null, type); 
            foreach (var type in TestTypes) Assert.IsNull(SchemeStorage.Get(null, type)); 
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        public void Clear_null_type_miss()
        {
            // Act
            SchemeStorage.Set(null, typeof(object), Instance);
            SchemeStorage.Clear(null, typeof(Type));
            Assert.IsNotNull(SchemeStorage.Get(null, typeof(object)));
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        public void Clear_target_type_hit()
        {
            foreach (var type in TestTypes) SchemeStorage.Set(typeof(Type), type, Instance); 
            foreach (var type in TestTypes) SchemeStorage.Clear(typeof(Type), type); 
            foreach (var type in TestTypes) Assert.IsNull(SchemeStorage.Get(typeof(Type), type)); 
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, LIST)]
        public void Clear_target_type_miss()
        {
            SchemeStorage.Set(typeof(Type), typeof(object), Instance);
            SchemeStorage.Clear(typeof(object), typeof(Type));
            Assert.IsNotNull(SchemeStorage.Get(typeof(Type), typeof(object)));
        }

        #endregion
    }
}
