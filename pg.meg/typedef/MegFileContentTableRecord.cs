using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using pg.util.interfaces;

[assembly: InternalsVisibleTo("pg.meg.test")]

namespace pg.meg.typedef
{
    internal class MegFileContentTableRecord : IBinaryFile, ISizeable, IComparable
    {
        protected readonly uint _crc32;
        protected uint _fileTableRecordIndex;
        protected readonly uint _fileSizeInBytes;
        protected uint _fileStartOffsetInBytes;
        protected readonly uint _fileNameTableIndex;

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

        public virtual byte[] GetBytes()
        {
            List<byte> b = new List<byte>();
            b.AddRange(BitConverter.GetBytes(_crc32));
            b.AddRange(BitConverter.GetBytes(_fileTableRecordIndex));
            b.AddRange(BitConverter.GetBytes(_fileSizeInBytes));
            b.AddRange(BitConverter.GetBytes(_fileStartOffsetInBytes));
            b.AddRange(BitConverter.GetBytes(_fileNameTableIndex));
            return b.ToArray();
        }

        public virtual uint Size()
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


    internal sealed class MegFileContentTableRecordV3 : MegFileContentTableRecord
    {
        private readonly ushort _flags;

        public MegFileContentTableRecordV3(ushort flags, uint crc32, uint fileTableRecordIndex, uint fileSizeInBytes,
            uint fileStartOffsetInBytes, ushort fileNameTableIndex) : base(crc32, fileNameTableIndex, fileSizeInBytes, fileStartOffsetInBytes, fileNameTableIndex)
        {
            _flags = flags;
        }

        public override byte[] GetBytes()
        {
            List<byte> b = new List<byte>();
            b.AddRange(BitConverter.GetBytes(_flags));
            b.AddRange(BitConverter.GetBytes(_crc32));
            b.AddRange(BitConverter.GetBytes(_fileTableRecordIndex));
            b.AddRange(BitConverter.GetBytes(_fileSizeInBytes));
            b.AddRange(BitConverter.GetBytes(_fileStartOffsetInBytes));
            b.AddRange(BitConverter.GetBytes(_fileNameTableIndex));
            //b.AddRange(Enumerable.Repeat((byte)0, 14)); // 14 bytes of padding
            return b.ToArray();
        }

        public override uint Size()
        {
            return sizeof(uint) * 4 + sizeof(ushort) * 2;// + sizeof(byte) * 14;
        }
    }
}
