using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace pg.meg.test
{
    [TestClass]
    public class MegFileUtilityTest
    {
        [TestMethod]
        [DataRow("I:\\Workspace\\pg.meg\\pg.meg.test\\test_data\\yvaw_metafiles.meg", 431)]
        public void GetMegFileHeaderSizeTest(string path, int expectedHeaderSize)
        {
            uint headerSize = MegFileUtility.GetMegFileHeaderSize(path);
            Assert.AreEqual(headerSize,Convert.ToUInt32(expectedHeaderSize));
        }

        [TestMethod]
        [DataRow("I:\\Workspace\\pg.meg\\pg.meg.test\\test_data\\yvaw_metafiles.meg", "I:\\Workspace\\pg.meg\\pg.meg.test\\test_data\\unpacked")]
        public void UnpackMegFileTest(string megFilePath, string targetDirectory)
        {
            MegFileUtility.UnpackMegFile(megFilePath, targetDirectory);
            Assert.IsTrue(Directory.Exists(targetDirectory));
        }
    }
}