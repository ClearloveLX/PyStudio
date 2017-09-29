using Microsoft.VisualStudio.TestTools.UnitTesting;
using PyStudio.Common;

namespace PyStudio.UnitTest
{
    [TestClass]
    public class BaseInfoUnitTest
    {
        private readonly DataClass _dataClass = new DataClass();
        [TestMethod]
        public void AreaCode()
        {
            var result = _dataClass.FormatCode(_dataClass.GetSerialNumber(null));
            Assert.AreEqual(result, "10001", true);
        } 
    }
}
