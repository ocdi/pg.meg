using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using pg.meg.builder;
using pg.meg.typedef;

namespace pg.meg.test
{
    [TestClass]
    public class MegFileBinaryFileBuilderUnitTest
    {
        private static string TEST_DATA_PATH_IN = "C:\\Workspace\\pg.meg\\pg.meg.test\\test_data\\yvaw_metafiles.meg";
        private static string TEST_DATA_PATH_OUT = "C:\\Workspace\\pg.meg\\pg.meg.test\\test_data\\yvaw_metafiles.txt";

        [TestMethod]
        public void MegBuilderBuildHeaderTest()
        {
            MegFileBinaryFileBuilder megFileBinaryFileBuilder = new MegFileBinaryFileBuilder();
            byte[] b = File.ReadAllBytes(TEST_DATA_PATH_IN);
            MegFile megFile = megFileBinaryFileBuilder.Build(b);
            using (StreamWriter w = new StreamWriter(TEST_DATA_PATH_OUT, true))
            {
                foreach (FileHolder containedFile in megFile.GetContainedFiles())
                {
                    string fileContent = Encoding.UTF8.GetString(containedFile.FileContent);
                    w.WriteLine($"\n<!-- ===== ===== ===== FILE: {containedFile.FileName} -->");
                    w.Write(fileContent);
                }
            }
            Assert.IsNotNull(megFile);
        }
    }
}
