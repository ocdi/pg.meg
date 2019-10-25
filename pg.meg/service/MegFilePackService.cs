using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using pg.meg.builder;
using pg.meg.builder.attributes;
using pg.meg.typedef;
using pg.meg.utility;
using pg.util;

[assembly: InternalsVisibleTo("pg.meg.test")]

namespace pg.meg.service
{
    internal sealed class MegFilePackService
    {
        internal static void PackMegFile(string fullyQualifiedMegFileSavePath, IEnumerable<string> absoluteFilePaths)
        {
            if (fullyQualifiedMegFileSavePath == null)
            {
                throw new ArgumentNullException(nameof(fullyQualifiedMegFileSavePath));
            }

            if (absoluteFilePaths == null)
            {
                throw new ArgumentNullException(nameof(absoluteFilePaths));
            }

            if (!PathUtility.IsValidDirectoryPath(fullyQualifiedMegFileSavePath))
            {
                throw new ArgumentException($"The provided path {fullyQualifiedMegFileSavePath} does not exist.");
            }

            IEnumerable<string> contentFiles = absoluteFilePaths.ToList();
            if (!contentFiles.Any())
            {
                throw new ArgumentException($"The meg file must contain at least one file.");
            }

            MegFileBinaryFileBuilder builder = new MegFileBinaryFileBuilder();
            MegFile megFile = builder.Build(new MegFileAttribute() {ContentFiles = contentFiles});
            using (BinaryWriter writer = new BinaryWriter(File.Open(fullyQualifiedMegFileSavePath, FileMode.Create)))
            {
                writer.Write(megFile.GetBytes());
            }

            foreach (string megFileFile in megFile.ContentTable
                .MegFileContentTableRecords.SelectMany(contentTableMegFileContentTableRecord => megFile.Files.Where(megFileFile => megFile
                    .LookupFileNameByIndex(
                        Convert.ToInt32(contentTableMegFileContentTableRecord.FileNameTableRecordIndex))
                    .Equals(MegFileContentUtility.ExtractFileNameForMegFile(megFileFile),
                        StringComparison.InvariantCultureIgnoreCase))))
            {
                using (FileStream readStream = File.OpenRead(megFileFile))
                {
                    using (FileStream writeStream = new FileStream(fullyQualifiedMegFileSavePath,
                        FileMode.Append,
                        FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead;
                        while ((bytesRead = readStream.Read(buffer, 0, 1024)) > 0)
                        {
                            writeStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }
    }
}
