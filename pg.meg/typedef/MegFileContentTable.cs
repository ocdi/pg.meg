using System.Collections.Generic;
using pg.util.interfaces;

namespace pg.meg.typedef
{
    public class MegFileContentTable : IBinaryFile
    {
        private readonly List<MegFileContentTableRecord> _megFileContentTableRecords;
        
        public MegFileContentTable(List<MegFileContentTableRecord> megFileContentTableRecords)
        {
            _megFileContentTableRecords = megFileContentTableRecords;
        }

        public List<MegFileContentTableRecord> MegFileContentTableRecords => _megFileContentTableRecords;

        public byte[] GetBytes()
        {
            List<byte> b = new List<byte>();
            foreach (MegFileContentTableRecord megFileContentTableRecord in _megFileContentTableRecords)
            {
                b.AddRange(megFileContentTableRecord.GetBytes());
            }
            return b.ToArray();
        }
    }
}
