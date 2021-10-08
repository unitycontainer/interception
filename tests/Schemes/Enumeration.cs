using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Unit.Tests
{
    public partial class Schemes
    {
        [TestMethod, TestProperty(INTERFACE, ENUMERATIONS)]
        public void Empty()
        {
            var enumeration = SchemeStorage.OfType(typeof(object));
            
            Assert.IsNotNull(enumeration);
            Assert.AreEqual(0, enumeration.ToArray().Length);
        }
    }
}
