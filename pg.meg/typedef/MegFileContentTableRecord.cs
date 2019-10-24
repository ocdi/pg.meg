using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using pg.util.interfaces;
[assembly: InternalsVisibleTo("pg.meg.test")]

namespace pg.meg.typedef
{
    internal sealed class MegFileContentTableRecord : IBinaryFile, ISizeable
    {
        private readonly uint _crc32;
        private readonly uint _fileTableRecordIndex;
        private readonly uint _fileSizeInBytes;
        private readonly uint _fileStartOffsetInBytes;
        private readonly uint _fileNameTableIndex;

        public MegFileContentTableRecord(uint crc32, uint fileTableRecordIndex, uint fileSizeInBytes, uint fileStartOffsetInBytes, uint fileNameTableIndex)
        {
            _crc32 = crc32;
            _fileTableRecordIndex = fileTableRecordIndex;
            _fileSizeInBytes = fileSizeInBytes;
            _fileStartOffsetInBytes = fileStartOffsetInBytes;
            _fileNameTableIndex = fileNameTableIndex;
        }

        public uint FileStartOffsetInBytes => _fileStartOffsetInBytes;
        public uint FileSizeInBytes => _fileSizeInBytes;

        public uint FileTableRecordIndex => _fileTableRecordIndex;
        public uint FileNameTableRecordIndex => _fileNameTableIndex;

        public byte[] GetBytes()
        {
            List<byte> b = new List<byte>();
            b.AddRange(BitConverter.GetBytes(_crc32));
            b.AddRange(BitConverter.GetBytes(_fileTableRecordIndex));
            b.AddRange(BitConverter.GetBytes(_fileSizeInBytes));
            b.AddRange(BitConverter.GetBytes(_fileStartOffsetInBytes));
            b.AddRange(BitConverter.GetBytes(_fileNameTableIndex));
            return b.ToArray();
        }

        public uint Size()
        {
            return sizeof(uint) * 5;
        }
    }
}
