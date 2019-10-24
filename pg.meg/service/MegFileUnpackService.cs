using System;
using System.IO;
using System.Runtime.CompilerServices;
using pg.meg.builder;
using pg.meg.typedef;
using pg.util;

[assembly: InternalsVisibleTo("pg.meg.test")]

namespace pg.meg.service
{
    internal sealed class MegFileUnpackService
    {
        internal static void UnpackMegFile(string megFilePath, string targetDirectory)
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
            using (BinaryReader reader = new BinaryReader(new FileStream(megFilePath, FileMode.Open)))
            {
                foreach (MegFileContentTableRecord megFileContentTableRecord in megFile.ContentTable.MegFileContentTableRecords)
                {
                    UnpackFile(targetDirectory, megFileContentTableRecord, megFile, reader);
                }
            }

        }

        private static void UnpackFile(string targetDirectory, MegFileContentTableRecord megFileContentTableRecord, MegFile megFile, BinaryReader reader)
        {
            byte[] file = new byte[megFileContentTableRecord.FileSizeInBytes];
            string fileName = megFile.FileNameTable.MegFileNameTableRecords[Convert.ToInt32(megFileContentTableRecord.FileNameTableRecordIndex)].FileName;
            reader.BaseStream.Seek(Convert.ToInt32(megFileContentTableRecord.FileStartOffsetInBytes), SeekOrigin.Begin);
            reader.Read(file, 0, file.Length);
            string path = Path.Combine(targetDirectory, fileName);
            PathUtility.CreatePath(Path.GetDirectoryName(path));
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(file);
            }
        }

        private static MegFile LoadMegFileFromDisk(string megFilePath)
        {
            uint headerSize = MegFileUtility.GetMegFileHeaderSize(megFilePath);
            byte[] megFileHeader = new byte[headerSize];
            using (BinaryReader reader = new BinaryReader(new FileStream(megFilePath, FileMode.Open)))
            {
                reader.Read(megFileHeader, 0, megFileHeader.Length);
            }

            MegFileBinaryFileBuilder builder = new MegFileBinaryFileBuilder();
            return builder.Build(megFileHeader);
        }
    }
}
