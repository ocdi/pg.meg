using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using pg.util.interfaces;
[assembly: InternalsVisibleTo("pg.meg.test")]

namespace pg.meg.typedef
{
    internal sealed class MegHeader : IBinaryFile, ISizeable
    {
        private readonly uint _numFileNames;
        private readonly uint _numFiles;

        public MegHeader(uint numFileNames, uint numFiles)
        {
            _numFileNames = numFileNames;
            _numFiles = numFiles;
        }

        public byte[] GetBytes()
        {
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
