using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PyStudio.UnitTest
{
    [TestClass]
    public class PublicMothodUnitTest
    {
        [TestMethod]
        public void ParseIntTest()
        {
            int number;
            bool result = int.TryParse("a123456",out number);
            Assert.AreEqual(number, 123456);
        }
    }
}
