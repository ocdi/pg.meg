using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using pg.meg.builder;
using pg.meg.typedef;

namespace pg.meg.test
{
    [TestClass]
    public class MegFileBinaryFileBuilderUnitTest
    {
        private static string TEST_DATA_PATH_IN = "I:\\Workspace\\pg.meg\\pg.meg.test\\test_data\\yvaw_metafiles.meg";
        private static string TEST_DATA_PATH_OUT = "I:\\Workspace\\pg.meg\\pg.meg.test\\test_data\\yvaw_metafiles.txt";

        [TestMethod]
        public void MegBuilderBuildHeaderTest()
        {
            MegFileBinaryFileBuilder megFileBinaryFileBuilder = new MegFileBinaryFileBuilder();
            byte[] b = File.ReadAllBytes(TEST_DATA_PATH_IN);
            MegFile megFile = megFileBinaryFileBuilder.Build(b);
            Assert.IsNotNull(megFile);
        }
    }
}
