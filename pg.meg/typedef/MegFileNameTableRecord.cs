using System;
using System.Collections.Generic;
using System.Text;
using pg.util.interfaces;

namespace pg.meg.typedef
{
    public class MegFileNameTableRecord : IBinaryFile
    {
        private readonly ushort _fileNameLength;
        private readonly string _fileName;

        public MegFileNameTableRecord(string fileName)
        {
            fileName = fileName.ToUpper();
            fileName = fileName.Replace("\0", string.Empty);
            int fileNameLength = fileName.Length;
            try
            {
                _fileNameLength = Convert.ToUInt16(fileNameLength);
                _fileName = fileName;
            }
            catch (OverflowException)
            {
                throw new OverflowException($"The filename {fileName} is too long to be inserted into a meg file. It may not exceed {ushort.MaxValue} characters.");
            }
        }

        public string FileName => _fileName;

        public byte[] GetBytes()
        {
            List<byte> b = new List<byte>();
            b.AddRange(BitConverter.GetBytes(_fileNameLength));
            b.AddRange(Encoding.ASCII.GetBytes(_fileName));
            return b.ToArray();
        }
    }
}
