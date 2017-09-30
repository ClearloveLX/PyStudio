using Microsoft.VisualStudio.TestTools.UnitTesting;
using PyStudio.Web.Extends;

namespace PyStudio.UnitTest
{
    [TestClass]
    public class PublicMothodUnitTest
    {
        [TestMethod]
        public void ParseIntTest()
        {
            int number;
            bool result = int.TryParse("a123456", out number);
            Assert.AreEqual(number, 123456);
        }

        [TestMethod]
        public void CharacterPrefixTest()
        {
            string sss = 10.CharacterPrefix();
            Assert.AreEqual(sss, "├────────────────────");
        }
    }
}
