using System.Collections.Generic;
using pg.util.interfaces;

namespace pg.meg.typedef
{
    public class MegFileByteContent : IBinaryFile
    {
        private List<FileHolder> _files;

        public MegFileByteContent(List<FileHolder> files)
        {
            _files = files;
        }

        public IEnumerable<FileHolder> GetContainedFiles()
        {
            return _files;
        }

        public byte[] GetBytes()
        {
            List<byte> b = new List<byte>();
            foreach (FileHolder fileHolder in _files)
            {
                b.AddRange(fileHolder.FileContent);
            }
            return b.ToArray();
        }
    }
}
