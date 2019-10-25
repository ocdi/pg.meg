using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using pg.util.interfaces;

[assembly: InternalsVisibleTo("pg.meg.test")]

namespace pg.meg.typedef
{
    internal sealed class MegFileContentTableRecord : IBinaryFile, ISizeable, IComparable
    {
        private readonly uint _crc32;
        private uint _fileTableRecordIndex;
        private readonly uint _fileSizeInBytes;
        private uint _fileStartOffsetInBytes;
        private readonly uint _fileNameTableIndex;

        public MegFileContentTableRecord(uint crc32, uint fileTableRecordIndex, uint fileSizeInBytes,
            uint fileStartOffsetInBytes, uint fileNameTableIndex)
        {
            _crc32 = crc32;
            _fileTableRecordIndex = fileTableRecordIndex;
            _fileSizeInBytes = fileSizeInBytes;
            _fileStartOffsetInBytes = fileStartOffsetInBytes;
            _fileNameTableIndex = fileNameTableIndex;
        }

        public uint FileStartOffsetInBytes
        {
            get => _fileStartOffsetInBytes;
            set => _fileStartOffsetInBytes = value;
        }

        public uint FileSizeInBytes => _fileSizeInBytes;

        public uint FileTableRecordIndex
        {
            get => _fileTableRecordIndex;
            set => _fileTableRecordIndex = value;
        }

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


        sealed class MegFileContentTableRecordComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null || y == null)
                {
                    return 0;
                }

                if (!(x is MegFileContentTableRecord a) || !(y is MegFileContentTableRecord b))
                {
                    return 0;
                }

                if (a._crc32 < b._crc32)
                {
                    return -1;
                }

                return 1;
            }
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            MegFileContentTableRecord b = obj as MegFileContentTableRecord;
            if (b != null && _crc32 > b._crc32)
            {
                return 1;
            }
            if (b != null && _crc32 < b._crc32)
            {
                return -1;
            }

            return 0;
        }
    }
}
