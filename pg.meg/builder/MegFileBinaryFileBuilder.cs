using System;
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
        private static int _CURRENT_OFFSET;
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

        public MegFile Build(MegFileAttribute attribute)
        {
            MegHeader megHeader = BuildMegHeader(attribute);
            MegFileNameTable megFileNameTable = BuildFileNameTable(attribute);
            uint currentFileSize = megHeader.Size() + megFileNameTable.Size();
            MegFileContentTable megFileContentTable = BuildFileContentTable(attribute, megFileNameTable, currentFileSize);

            MegFile megFile = new MegFile(megHeader, megFileNameTable, megFileContentTable) {Files = attribute.ContentFiles.ToList()};

            return megFile;
        }

        private MegFileContentTable BuildFileContentTable(MegFileAttribute attribute, MegFileNameTable megFileNameTable, uint currentFileSize)
        {
            uint totalFileHeaderSize = currentFileSize + Convert.ToUInt32(attribute.ContentFiles.Count() * new MegFileContentTableRecord(0, 0, 0, 0, 0).Size());
            List<MegFileContentTableRecord> megFileContentTable = new List<MegFileContentTableRecord>();
            List<string> files = attribute.ContentFiles.ToList();
            for (int i = 0; i < attribute.ContentFiles.Count(); i++)
            {
                uint crc32 = ChecksumUtility.GetChecksum(megFileNameTable.MegFileNameTableRecords[i].FileName);
                uint fileTableRecordIndex = Convert.ToUInt32(i);
                FileInfo fileInfo = new FileInfo(files[i]);
                uint fileSizeInBytes = Convert.ToUInt32(fileInfo.Length);
                
                uint fileNameTableIndex = Convert.ToUInt32(i);
                
                MegFileContentTableRecord megFileContentTableRecord = new MegFileContentTableRecord(crc32, fileTableRecordIndex, fileSizeInBytes, totalFileHeaderSize, fileNameTableIndex);
                totalFileHeaderSize += fileSizeInBytes;
                
                megFileContentTable.Add(megFileContentTableRecord);
            }
            
            return new MegFileContentTable(megFileContentTable);
        }

        private static MegHeader BuildMegHeader(MegFileAttribute attribute)
        {
            uint numFiles = Convert.ToUInt32(attribute.ContentFiles.Count());
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
            _CURRENT_OFFSET = FILE_NAME_TABLE_STARTING_OFFSET;
            for (uint i = 0; i < _numberOfFiles; i++)
            {
                ushort fileNameLength = BitConverter.ToUInt16(megFileBytes, _CURRENT_OFFSET);
                string fileName = Encoding.ASCII.GetString(megFileBytes, _CURRENT_OFFSET + sizeof(ushort), fileNameLength);
                _CURRENT_OFFSET = _CURRENT_OFFSET + sizeof(ushort) + fileNameLength;
                table.Add(new MegFileNameTableRecord(fileName));
            }
            return new MegFileNameTable(table);
        }

        private static MegFileNameTable BuildFileNameTable(MegFileAttribute megFileBytes)
        {
            List<string> actualFiles = new List<string>();
            List<MegFileNameTableRecord> megFileNameList = new List<MegFileNameTableRecord>();
            foreach (string file in megFileBytes.ContentFiles)
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
            megFileBytes.ContentFiles = actualFiles;
            return new MegFileNameTable(megFileNameList);
        }

        private MegFileContentTable BuildFileContentTable(byte[] megFileBytes)
        {
            List<MegFileContentTableRecord> megFileContentTableRecords = new List<MegFileContentTableRecord>();

            for (uint i = 0; i < _numberOfFiles; i++)
            {
                uint crc32 = BitConverter.ToUInt32(megFileBytes, _CURRENT_OFFSET);
                _CURRENT_OFFSET += sizeof(uint);
                uint fileTableRecordIndex = BitConverter.ToUInt32(megFileBytes, _CURRENT_OFFSET);
                _CURRENT_OFFSET += sizeof(uint);
                uint fileSizeInBytes = BitConverter.ToUInt32(megFileBytes, _CURRENT_OFFSET);
                _CURRENT_OFFSET += sizeof(uint);
                uint fileStartOffsetInBytes = BitConverter.ToUInt32(megFileBytes, _CURRENT_OFFSET);
                _CURRENT_OFFSET += sizeof(uint);
                uint fileNameTableIndex = BitConverter.ToUInt32(megFileBytes, _CURRENT_OFFSET);
                _CURRENT_OFFSET += sizeof(uint);
                megFileContentTableRecords.Add(new MegFileContentTableRecord(
                    crc32,
                    fileTableRecordIndex,
                    fileSizeInBytes,
                    fileStartOffsetInBytes,
                    fileNameTableIndex));
            }

            return new MegFileContentTable(megFileContentTableRecords);
        }
    }
}
