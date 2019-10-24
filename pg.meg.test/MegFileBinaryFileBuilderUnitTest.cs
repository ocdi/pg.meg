using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using pg.meg.builder;
using pg.meg.typedef;

namespace pg.meg.test
{
    [TestClass]
    public class MegFileBinaryFileBuilderUnitTest
    {
        private static readonly string TEST_DATA_PATH_IN = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "test_data\\eaw_patch_2.meg"));

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
