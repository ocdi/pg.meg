using System.Collections.Generic;
using System.Runtime.CompilerServices;
using pg.util.interfaces;
[assembly: InternalsVisibleTo("pg.meg.test")]

namespace pg.meg.builder.attributes
{
    public sealed class MegFileAttribute : IBuilderAttribute
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
