using System;
using System.IO;
using pg.util;
using pg.util.enumerations;

namespace pg.meg.typedef
{
    public class FileHolder
    {
        private string _fileName;
        private byte[] _fileContent;
        private PetroglyphFileType _type;

        public FileHolder(string fileName, byte[] fileContent)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileContent == null || fileContent.Length < 1)
            {
                throw new ArgumentNullException(nameof(fileContent));
            }
            _fileName = fileName;
            _fileContent = fileContent;
            _type = FileTypeUtility.GetPetroglyphFileTypeByExtension(Path.GetExtension(_fileName)?.Replace(".",string.Empty));
        }


        public string FileName => _fileName;

        public byte[] FileContent => _fileContent;

        public PetroglyphFileType Type => _type;
    }
}
