using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using pg.meg.service;
using pg.meg.typedef;

[assembly: InternalsVisibleTo("pg.meg.test")]

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
            PackMegFile(Path.GetFullPath(Path.Combine(megFileSavePath, megFileName)), absoluteFilePaths);
        }

        /// <summary>Packs the specified files into a specified meg file.</summary>
        /// <param name="fullyQualifiedMegFileSavePath">The fully qualified path to save the meg file under. This includes the meg file's fully qualified name.</param>
        /// <param name="absoluteFilePaths">The absolute file paths to the files the meg file should include.</param>
        public static void PackMegFile(string fullyQualifiedMegFileSavePath, IEnumerable<string> absoluteFilePaths)
        {
            MegFilePackService.PackMegFile(fullyQualifiedMegFileSavePath, absoluteFilePaths);
        }

        /// <summary>Unpacks the contents of a specified meg file into a given directory.</summary>
        /// <param name="megFilePath">The path to the meg file to unpack.</param>
        /// <param name="targetDirectory">The path to the directory the meg file should be unpacked to.</param>
        public static void UnpackMegFile(string megFilePath, string targetDirectory)
        {
            MegFileUnpackService.UnpackMegFile(megFilePath, targetDirectory);
        }

        internal static uint GetContainedFileCount(string megFilePath)
        {
            byte[] test = new byte[sizeof(uint)];
            using (BinaryReader reader = new BinaryReader(new FileStream(megFilePath, FileMode.Open)))
            {
                reader.Read(test, 0, sizeof(uint));
            }
            return BitConverter.ToUInt32(test, 0);
        }

        internal static uint GetMegFileHeaderSize(string megFilePath)
        {
            uint headerSize = 0;
            using (BinaryReader reader = new BinaryReader(new FileStream(megFilePath, FileMode.Open)))
            {
                uint containedFiles = reader.ReadUInt32();
                uint currentOffset = sizeof(uint) * 2;
                for (uint i = 0; i < containedFiles; i++)
                {
                    reader.BaseStream.Seek(currentOffset, SeekOrigin.Begin);
                    ushort fileNameLenght = reader.ReadUInt16();
                    currentOffset += Convert.ToUInt32(sizeof(ushort) + fileNameLenght);
                }

                headerSize += currentOffset;

                uint megContentTableRecordSize = new MegFileContentTableRecord(0, 0, 0, 0, 0).Size();
                headerSize += megContentTableRecordSize * containedFiles;
            }

            return headerSize;
        }
    }
}
