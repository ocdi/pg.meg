using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using pg.meg.builder.attributes;
using pg.meg.exceptions;
using pg.meg.typedef;
using pg.meg.utility;
using pg.util;
using pg.util.interfaces;

[assembly: InternalsVisibleTo("pg.meg.test")]

namespace pg.meg.builder
{
    internal sealed class MegFileBinaryFileBuilder : IBinaryFileBuilder<MegFile, MegFileAttribute>
    {
        private const int HEADER_STARTING_OFFSET = 0;
        private const int HEADER_NUMBER_OF_FILES_OFFSET = 4;
        private const int FILE_NAME_TABLE_STARTING_OFFSET = 8;
        private int _currentOffset;
        private uint _numberOfFiles;

        public MegFile Build(byte[] megFileBytes)
        {
            if (megFileBytes == null || megFileBytes.Length < 1)
            {
                throw new ArgumentNullException(nameof(megFileBytes), "The provided file is empty.");
            }

            MegHeader header = BuildMegHeader(megFileBytes);
            MegFileNameTable megFileNameTable = BuildFileNameTable(megFileBytes);
            MegFileContentTable megFileContentTable = BuildFileContentTable(megFileBytes);
            return new MegFile(header, megFileNameTable, megFileContentTable);
        }

        public MegFile Build(MegFileAttribute megFileAttribute)
        {
            MegHeader megHeader = BuildMegHeader(megFileAttribute);
            MegFileNameTable megFileNameTable = BuildFileNameTable(megFileAttribute);
            //uint currentFileSize = megHeader.Size() + megFileNameTable.Size();
            MegFileContentTable megFileContentTable =
                BuildFileContentTables(megFileAttribute, megFileNameTable);
                //BuildFileContentTable(megFileAttribute, megFileNameTable, currentFileSize);
            MegFile megFile =
                new MegFile(megHeader, megFileNameTable, megFileContentTable) {Files = megFileAttribute.ContentFiles.ToList()};

            return megFile;
        }

        private static MegHeader BuildMegHeader(MegFileAttribute megFileAttribute)
        {
            uint numFiles = Convert.ToUInt32(megFileAttribute.ContentFiles.Count());
            return new MegHeader(numFiles, numFiles);
        }

        private MegHeader BuildMegHeader(byte[] megFileBytes)
        {
            uint numFileNames = BitConverter.ToUInt32(megFileBytes, HEADER_STARTING_OFFSET);
            uint numFiles = BitConverter.ToUInt32(megFileBytes, HEADER_NUMBER_OF_FILES_OFFSET);
            if (numFiles != numFileNames)
            {
                throw new MegFileMalformedException(
                    $"The number of file names has to be identical to the number of files. File names: {numFileNames} Number of files: {numFiles}.");
            }

            _numberOfFiles = numFiles;
            return new MegHeader(numFileNames, numFiles);
        }

        private MegFileNameTable BuildFileNameTable(byte[] megFileBytes)
        {
            List<MegFileNameTableRecord> table = new List<MegFileNameTableRecord>();
            _currentOffset = FILE_NAME_TABLE_STARTING_OFFSET;
            for (uint i = 0; i < _numberOfFiles; i++)
            {
                ushort fileNameLength = BitConverter.ToUInt16(megFileBytes, _currentOffset);
                string fileName =
                    Encoding.ASCII.GetString(megFileBytes, _currentOffset + sizeof(ushort), fileNameLength);
                _currentOffset = _currentOffset + sizeof(ushort) + fileNameLength;
                table.Add(new MegFileNameTableRecord(fileName));
            }

            return new MegFileNameTable(table);
        }

        private static MegFileNameTable BuildFileNameTable(MegFileAttribute megFileAttribute)
        {
            List<string> actualFiles = new List<string>();
            List<MegFileNameTableRecord> megFileNameList = new List<MegFileNameTableRecord>();
            foreach (string file in megFileAttribute.ContentFiles)
            {
                try
                {
                    string fileName = MegFileContentUtility.ExtractFileNameForMegFile(file);
                    MegFileNameTableRecord megFileNameTableRecord = new MegFileNameTableRecord(fileName);
                    megFileNameList.Add(megFileNameTableRecord);
                    actualFiles.Add(file);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"File {file} is invalid. {e}");
                }
            }

            megFileAttribute.ContentFiles = actualFiles;
            return new MegFileNameTable(megFileNameList);
        }

        private MegFileContentTable BuildFileContentTable(byte[] megFileBytes)
        {
            List<MegFileContentTableRecord> megFileContentTableRecords = new List<MegFileContentTableRecord>();

            for (uint i = 0; i < _numberOfFiles; i++)
            {
                uint crc32 = BitConverter.ToUInt32(megFileBytes, _currentOffset);
                _currentOffset += sizeof(uint);
                uint fileTableRecordIndex = BitConverter.ToUInt32(megFileBytes, _currentOffset);
                _currentOffset += sizeof(uint);
                uint fileSizeInBytes = BitConverter.ToUInt32(megFileBytes, _currentOffset);
                _currentOffset += sizeof(uint);
                uint fileStartOffsetInBytes = BitConverter.ToUInt32(megFileBytes, _currentOffset);
                _currentOffset += sizeof(uint);
                uint fileNameTableIndex = BitConverter.ToUInt32(megFileBytes, _currentOffset);
                _currentOffset += sizeof(uint);
                megFileContentTableRecords.Add(new MegFileContentTableRecord(
                    crc32,
                    fileTableRecordIndex,
                    fileSizeInBytes,
                    fileStartOffsetInBytes,
                    fileNameTableIndex));
            }

            return new MegFileContentTable(megFileContentTableRecords);
        }

        private MegFileContentTable BuildFileContentTables(MegFileAttribute megFileAttribute,
            MegFileNameTable megFileNameTable)
        {
            List<string> absoluteFilePaths = megFileAttribute.ContentFiles.ToList();
            List<MegFileContentTableRecord> megFileContentList = new List<MegFileContentTableRecord>();
            uint currentOffset = new MegHeader(0,0).Size();
            currentOffset += megFileNameTable.Size();

            for (int i = 0; i < megFileNameTable.MegFileNameTableRecords.Count; i++)
            {
                uint crc32 = ChecksumUtility.GetChecksum(megFileNameTable.MegFileNameTableRecords[i].FileName);
                uint fileSizeInBytes = Convert.ToUInt32(new FileInfo(absoluteFilePaths[i]).Length);
                uint fileNameTableIndex = Convert.ToUInt32(i);
                MegFileContentTableRecord megFileContentTableRecord = new MegFileContentTableRecord(crc32, 0, fileSizeInBytes, 0, fileNameTableIndex);
                megFileContentList.Add(megFileContentTableRecord);
                currentOffset += megFileContentTableRecord.Size();
            }

            megFileContentList.Sort();

            for (int i = 0; i < megFileContentList.Count; i++)
            {
                megFileContentList[i].FileTableRecordIndex = Convert.ToUInt32(i);
                megFileContentList[i].FileStartOffsetInBytes = currentOffset;
                currentOffset += megFileContentList[i].FileSizeInBytes;
            }

            return new MegFileContentTable(megFileContentList);
        }
    }
}
