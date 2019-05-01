using System;
using System.IO;
using System.Runtime.CompilerServices;
using pg.meg.typedef;
using pg.util;

[assembly: InternalsVisibleTo("pg.meg.test")]

namespace pg.meg.service
{
    internal sealed class MegFileUnpackService
    {
        internal void UnpackMegFile(string megFilePath, string targetDirectory)
        {
            bool megFilePathExists = File.Exists(megFilePath);
            bool targetDirectoryExists = PathUtility.CreatePath(targetDirectory);
            if (!megFilePathExists)
            {
                throw new ArgumentNullException($"The specified *.meg file at {megFilePath} does not exist.");
            }

            if (!targetDirectoryExists)
            {
                throw new ArgumentException($"The directory {targetDirectory} could not be found or created. The provided path is invalid."); 
            }

            MegFile megFile = LoadMegFileFromDisk(megFilePath);

        }

        private MegFile LoadMegFileFromDisk(string megFilePath)
        {
            uint fileCount = MegFileUtility.GetContainedFileCount(megFilePath);
            return null;
        }
    }
}