using System.Collections.Generic;
using pg.util.interfaces;

namespace pg.meg.typedef
{
    public sealed class MegFile : IBinaryFile
    {
        private readonly MegHeader _header;
        private readonly MegFileNameTable _fileNameTable;
        private readonly MegFileContentTable _megFileContentTable;
        private readonly MegFileByteContent _megFileByteContent;

        public MegFile(MegHeader header, MegFileNameTable fileNameTable, MegFileContentTable megFileContentTable, MegFileByteContent megFileByteContent)
        {
            _header = header;
            _fileNameTable = fileNameTable;
            _megFileContentTable = megFileContentTable;
            _megFileByteContent = megFileByteContent;
        }

        public IEnumerable<FileHolder> GetContainedFiles()
        {
            return _megFileByteContent.GetContainedFiles();
        }

        public byte[] GetBytes()
        {
            List<byte> b = new List<byte>();
            b.AddRange(_header.GetBytes());
            b.AddRange(_fileNameTable.GetBytes());
            b.AddRange(_megFileContentTable.GetBytes());
            b.AddRange(_megFileByteContent.GetBytes());
            return b.ToArray();
        }
    }
}
