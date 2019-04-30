using System;
using System.Collections.Generic;

namespace pg.meg
{
    public sealed class MegFileUtility
    {
        /// <summary>Packs the specified files into a specified meg file.</summary>
        /// <param name="megFileName">The name to save the meg under.</param>
        /// <param name="megFileSavePath">The path to the directory to save the meg file under.</param>
        /// <param name="absoluteFilePaths">The absolute file paths to the files the meg file should include.</param>
        public static void PackMegFile(string megFileName, string megFileSavePath, IEnumerable<string> absoluteFilePaths)
        {
            throw new NotImplementedException();
        }

        /// <summary>Packs the specified files into a specified meg file.</summary>
        /// <param name="fullyQualifiedMegFileSavePath">The fully qualified path to save the meg file under. This includes the meg file's fully qualified name.</param>
        /// <param name="absoluteFilePaths">The absolute file paths to the files the meg file should include.</param>
        public static void PackMegFile(string fullyQualifiedMegFileSavePath, IEnumerable<string> absoluteFilePaths)
        {
            throw new NotImplementedException();
        }

        /// <summary>Unpacks the contents of a specified meg file into a given directory.</summary>
        /// <param name="megFilePath">The path to the meg file to unpack.</param>
        /// <param name="targetDirectory">The path to the directory the meg file should be unpacked to.</param>
        public static void UnpackMegFile(string megFilePath, string targetDirectory)
        {
            throw new NotImplementedException();
        }
    }
}
