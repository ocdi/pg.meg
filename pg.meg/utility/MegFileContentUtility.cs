using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using pg.meg.exceptions;
using pg.util.exceptions;
[assembly: InternalsVisibleTo("pg.meg.test")]

namespace pg.meg.utility
{
    internal sealed class MegFileContentUtility
    {
        private const string DATA_PATH_REG_EX = @"(.*)(data)(.*)";

        internal static string ExtractFileNameForMegFile(string megFileName)
        {
            if (string.IsNullOrEmpty(megFileName))
            {
                throw new AttributeNullException();
            }
            Regex regularExpression = new Regex(DATA_PATH_REG_EX, RegexOptions.IgnoreCase);
            Match match = regularExpression.Match(megFileName);

            if (match.Groups.Count < 4)
            {
                throw new InvalidFileNameException($"The provided file name {megFileName} is invalid.");
            }
            string path = match.Groups[3].Value;
            path = path.Replace("\\", "/");
            path = path.ToUpper();
            path = $"DATA/{path}";
            
            return path;
        }
    }
}