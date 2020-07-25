using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using pg.meg.builder;
using pg.meg.exceptions;
using pg.util.interfaces;
[assembly: InternalsVisibleTo("pg.meg.test")]

namespace pg.meg.typedef
{
    internal sealed class MegHeader : IBinaryFile, ISizeable
    {
        internal const int HEADER_STARTING_OFFSET = 0;
        internal const int HEADER_NUMBER_OF_FILES_OFFSET = 4;
        internal const int FILE_NAME_TABLE_STARTING_OFFSET = 8;

        internal const int HEADER_NUMBER_OF_FILE_NAMES_OFFSET_V3 = 0xC;
        internal const int HEADER_NUMBER_OF_FILES_OFFSET_V3 = 0x10;
        internal const int HEADER_FILE_TABLE_SIZE_OFFSET_V3 = 0x14;
        internal const int FILE_NAME_TABLE_STARTING_OFFSET_V3 = 0x18;


        private readonly uint _numFileNames;
        private readonly uint _numFiles;
        internal readonly MegFileVersion _fileVersion;
        internal readonly int _fileTableOffset;

        public MegHeader(uint numFileNames, uint numFiles)
        {
            _numFileNames = numFileNames;
            _numFiles = numFiles;
            _fileTableOffset = FILE_NAME_TABLE_STARTING_OFFSET;
            _fileVersion = MegFileVersion.V1;
        }
        public MegHeader(uint numFileNames, uint numFiles, MegFileVersion fileVersion, int fileTableOffset)
        {
            _numFileNames = numFileNames;
            _numFiles = numFiles;
            _fileVersion = fileVersion;
            _fileTableOffset = fileTableOffset;
        }

        public byte[] GetBytes()
        {
            if (_fileVersion == MegFileVersion.V3)
            {
                throw new InvalidOperationException("Writing a v3 file is not currently supported");
            }

            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(_numFileNames));
            bytes.AddRange(BitConverter.GetBytes(_numFiles));
            return bytes.ToArray();
        }

        public uint Size()
        {
            return sizeof(uint) * 2;
        }
    }
}
