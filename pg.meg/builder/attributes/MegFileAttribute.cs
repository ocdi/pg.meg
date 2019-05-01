using System.Collections.Generic;
using pg.util.interfaces;

namespace pg.meg.builder.attributes
{
    internal sealed class MegFileAttribute : IBuilderAttribute
    {
        private IEnumerable<string> _contentFiles;
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public IEnumerable<string> ContentFiles
        {
            get => _contentFiles ?? new List<string>();
            set => _contentFiles = value;
        }
    }
}