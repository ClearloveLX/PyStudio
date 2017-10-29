using Microsoft.VisualStudio.TestTools.UnitTesting;
using static PyStudio.Common.Helper.EnumHelper;

namespace PyStudio.UnitTest
{
    [TestClass]
    public class OthodUnitTest
    {
        [TestMethod]
        public void GetEmName()
        {
            string result = EmLogStatus.注册.ToString();
            Assert.AreEqual(result, "注册");
        }
    }
}
