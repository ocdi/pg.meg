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

    // there is a v2 but it is unsupported
    internal enum MegFileVersion
    {
        V1,
        V3
    }
    internal sealed class MegFileBinaryFileBuilder : IBinaryFileBuilder<MegFile, MegFileAttribute>
    {


        private int _currentOffset;
        private uint _numberOfFiles;

        public MegFile Build(byte[] megFileBytes)
        {
            if (megFileBytes == null || megFileBytes.Length < 1)
            {
                throw new ArgumentNullException(nameof(megFileBytes), "The provided file is empty.");
            }

            MegHeader header = BuildMegHeader(megFileBytes);
            MegFileNameTable megFileNameTable = BuildFileNameTable(header, megFileBytes);
            MegFileContentTable megFileContentTable = BuildFileContentTable(header, megFileBytes);
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
            uint numFileNames = BitConverter.ToUInt32(megFileBytes, MegHeader.HEADER_STARTING_OFFSET);
            uint numFiles = BitConverter.ToUInt32(megFileBytes, MegHeader.HEADER_NUMBER_OF_FILES_OFFSET);
            var version = MegFileVersion.V1;
            var fileNameTableOffset = MegHeader.FILE_NAME_TABLE_STARTING_OFFSET;
            

            if (numFileNames == MegFileUtility.FORMAT_V3_ENCRYPTED)
            {
                throw new MegFileMalformedException("Encrypted V3 files are unsupported");
            }
            if (numFileNames == MegFileUtility.FORMAT_V2_OR_V3)
            {
                if (numFiles != MegFileUtility.FORMAT_ID2) throw new MegFileMalformedException("File identification header bytes are incorrect");

                numFileNames = BitConverter.ToUInt32(megFileBytes, MegHeader.HEADER_NUMBER_OF_FILE_NAMES_OFFSET_V3);
                numFiles = BitConverter.ToUInt32(megFileBytes, MegHeader.HEADER_NUMBER_OF_FILES_OFFSET_V3);
                fileNameTableOffset = MegHeader.FILE_NAME_TABLE_STARTING_OFFSET_V3;

                // todo: do we need this?
                var fileTableOffset = (int)BitConverter.ToUInt32(megFileBytes, MegHeader.HEADER_FILE_TABLE_SIZE_OFFSET_V3) 
                    + MegHeader.FILE_NAME_TABLE_STARTING_OFFSET_V3; // the main header is 0x18 in size
                version = MegFileVersion.V3;
            }

            // according to https://modtools.petrolution.net/docs/MegFileFormat
            // this isn't true
            if (numFiles != numFileNames)
            {
                throw new MegFileMalformedException(
                    $"The number of file names has to be identical to the number of files. File names: {numFileNames} Number of files: {numFiles}.");
            }

            _numberOfFiles = numFiles;
            return new MegHeader(numFileNames, numFiles, version, fileNameTableOffset);
        }

        private MegFileNameTable BuildFileNameTable(MegHeader header, byte[] megFileBytes)
        {
            List<MegFileNameTableRecord> table = new List<MegFileNameTableRecord>();
            _currentOffset = header._fileTableOffset;
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

        private MegFileContentTable BuildFileContentTable(MegHeader header, byte[] megFileBytes)
        {
            List<MegFileContentTableRecord> megFileContentTableRecords = new List<MegFileContentTableRecord>();

            var isV3 = header._fileVersion == MegFileVersion.V3;

            for (uint i = 0; i < _numberOfFiles; i++)
            {
                
                if (isV3)
                {
                    ushort flags = BitConverter.ToUInt16(megFileBytes, _currentOffset);
                    if (flags != 0) throw new MegFileMalformedException($"Encrypted files are not supported. The file at index {i} was marked as encrypted.");
                    _currentOffset += sizeof(ushort);
                }

                uint crc32 = BitConverter.ToUInt32(megFileBytes, _currentOffset);
                _currentOffset += sizeof(uint);
                uint fileTableRecordIndex = BitConverter.ToUInt32(megFileBytes, _currentOffset);
                _currentOffset += sizeof(uint);
                uint fileSizeInBytes = BitConverter.ToUInt32(megFileBytes, _currentOffset);
                _currentOffset += sizeof(uint);
                uint fileStartOffsetInBytes = BitConverter.ToUInt32(megFileBytes, _currentOffset);
                _currentOffset += sizeof(uint);
                
                uint fileNameTableIndex = isV3 
                    ? BitConverter.ToUInt16(megFileBytes, _currentOffset) // v3 uses smaller index size
                    : BitConverter.ToUInt32(megFileBytes, _currentOffset);

                _currentOffset += isV3 
                    ? sizeof(ushort) 
                    : sizeof(uint);

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
