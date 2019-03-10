using System.Collections.Generic;
using pg.util.interfaces;

namespace pg.meg.typedef
{
    public sealed class MegFileNameTable : IBinaryFile
    {
        private readonly List<MegFileNameTableRecord> _megFileNameTableRecords;

        public MegFileNameTable(List<MegFileNameTableRecord> megFileNameTableRecords)
        {
            _megFileNameTableRecords = megFileNameTableRecords;
        }

        public List<MegFileNameTableRecord> MegFileNameTableRecords => _megFileNameTableRecords;

        public byte[] GetBytes()
        {
            List<byte> b = new List<byte>();
            foreach (MegFileNameTableRecord megFileNameTableRecord in _megFileNameTableRecords)
            {
                b.AddRange(megFileNameTableRecord.GetBytes());
            }

            return b.ToArray();
        }
    }
}
