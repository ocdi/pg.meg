using System;
using System.Collections.Generic;
using pg.util.interfaces;

namespace pg.meg.typedef
{
    public sealed class MegHeader : IBinaryFile
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
    }
}
