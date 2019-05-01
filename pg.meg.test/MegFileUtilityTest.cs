using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace pg.meg.test
{
    [TestClass]
    public class MegFileUtilityTest
    {
        [TestMethod]
        [DataRow("I:\\Workspace\\pg.meg\\pg.meg.test\\test_data\\yvaw_metafiles.meg", 431)]
        public void GetMegFileHeaderSizeTest(string path, uint expectedHeaderSize)
        {
            uint headerSize = 0;
            headerSize = MegFileUtility.GetMegFileHeaderSize(path);
            Assert.Equals(headerSize, expectedHeaderSize);
        }
    }
}