using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using pg.meg.exceptions;
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

        internal const uint FORMAT_V2_OR_V3 = 0xFFFFFFFF;
        internal const uint FORMAT_V3_ENCRYPTED = 0x8FFFFFFF;
        internal const uint FORMAT_ID2 = 0x3F7D70A4;

        internal static uint GetMegFileHeaderSize(string megFilePath)
        {
            uint? dataStart = null;
            uint numFiles;
            bool newFormat = false;
            uint? filenameTableSize = null;
            long headerSize = 0;
            using (BinaryReader reader = new BinaryReader(new FileStream(megFilePath, FileMode.Open)))
            {
                uint containedFiles = reader.ReadUInt32();
                if (containedFiles.Equals(FORMAT_V2_OR_V3)) // could be v2 or v3
                {
                    var check = reader.ReadUInt32();
                    if (!check.Equals(FORMAT_ID2)) // confirmed v2/3 format
                    {
                        throw new MegFileMalformedException($"{megFilePath} has incorrect header for version 2 or 3.");
                    }
                    dataStart = reader.ReadUInt32();
                    // now we have the file count
                    containedFiles = reader.ReadUInt32();
                    numFiles = reader.ReadUInt32();
                    filenameTableSize = reader.ReadUInt32();
                    newFormat = true;
                }
                else if (containedFiles.Equals(FORMAT_V3_ENCRYPTED))
                {
                    throw new MegFileMalformedException($"{megFilePath} encrypted version 3 is not supported at this time.");
                } 
                else
                {
                    numFiles = reader.ReadUInt32();
                }
                var fileNameTableOffset = (uint)reader.BaseStream.Position;
                var currentOffset = reader.BaseStream.Position;
                for (uint i = 0; i < containedFiles; i++)
                {
                    reader.BaseStream.Seek(currentOffset, SeekOrigin.Begin);
                    ushort fileNameLenght = reader.ReadUInt16();
                    currentOffset += (sizeof(ushort) + fileNameLenght);
                }

                if (newFormat && !(filenameTableSize + fileNameTableOffset).Equals((uint)currentOffset))
                {
                    throw new MegFileMalformedException($"{megFilePath} had inconsistent filename table size - expected {filenameTableSize + fileNameTableOffset} but we are at {currentOffset} (could be V2 which is not supported)");
                }
                headerSize += currentOffset;

                uint megContentTableRecordSize = !newFormat ? new MegFileContentTableRecord(0, 0, 0, 0, 0).Size() : new MegFileContentTableRecordV3(0, 0, 0, 0, 0, 0).Size();
                headerSize += megContentTableRecordSize * numFiles;
            }
            if (dataStart != null && !dataStart.Equals((uint)headerSize))
            {
                throw new MegFileMalformedException($"{megFilePath} had different dataStart ({dataStart}) vs calculated ({headerSize})");
            }

            return (uint)headerSize;
        }
    }
}
