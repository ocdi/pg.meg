using System;
using System.Collections.Generic;
using System.Text;
using pg.meg.exceptions;
using pg.meg.typedef;
using pg.util.interfaces;

namespace pg.meg.builder
{
    public class MegFileBinaryFileBuilder : IBinaryFileBuilder<MegFile>
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
            MegFileByteContent megFileByteContent = BuildMegFileByteContentHolder(megFileBytes, megFileNameTable, megFileContentTable);
            return new MegFile(header, megFileNameTable, megFileContentTable, megFileByteContent);
        }

        public byte[] Build(object payload)
        {
            throw new NotImplementedException();
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

        private MegFileByteContent BuildMegFileByteContentHolder(IReadOnlyList<byte> megFileBytes,
            MegFileNameTable megFileNameTable, MegFileContentTable megFileContentTable)
        {
            List<FileHolder> files = new List<FileHolder>();
            for (uint i = 0; i < _numberOfFiles; i++)
            {
                List<byte> fileContent = new List<byte>();
                for (uint j = megFileContentTable.MegFileContentTableRecords[Convert.ToInt32(i)].FileStartOffsetInBytes;
                    j < megFileContentTable.MegFileContentTableRecords[Convert.ToInt32(i)].FileStartOffsetInBytes + megFileContentTable.MegFileContentTableRecords[Convert.ToInt32(i)].FileSizeInBytes;
                    j++)
                {
                    fileContent.Add(megFileBytes[Convert.ToInt32(j)]);
                }
                files.Add(new FileHolder(megFileNameTable.MegFileNameTableRecords[Convert.ToInt32(i)].FileName, fileContent.ToArray()));
            }
            return new MegFileByteContent(files);
        }
    }
}
