using System;
using System.IO;
using System.Reflection;
using System.Text;
using Arrowgene.Buffers;

namespace Arrowgene.Baf.Server.Common
{
    public static class Util
    {
        public static readonly Encoding EncodingSimpChinese;

        static Util()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            EncodingSimpChinese = Encoding.GetEncoding("gb2312");
        }

        public static DirectoryInfo ExecutingDirectory()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                return null;
            }

            string path = assembly.CodeBase;
            Uri uri = new Uri(path);
            string directory = Path.GetDirectoryName(uri.LocalPath);
            if (directory == null)
            {
                return null;
            }

            return new DirectoryInfo(directory);
        }

        public static string ToHexString(byte[] data, char? separator = null)
        {
            StringBuilder sb = new StringBuilder();
            int len = data.Length;
            for (int i = 0; i < len; i++)
            {
                sb.Append(data[i].ToString("X2"));
                if (separator != null && i < len - 1)
                {
                    sb.Append(separator);
                }
            }

            return sb.ToString();
        }

        public static string HexDump(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length;

            char[] hexChars = "0123456789ABCDEF".ToCharArray();
            int firstHexColumn =
                8 // 8 characters for the address
                + 3; // 3 spaces

            int firstCharColumn = firstHexColumn
                                  + bytesPerLine * 3 // - 2 digit for the hexadecimal value and 1 space
                                  + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                                  + 2; // 2 spaces 

            int lineLength = firstCharColumn
                             + bytesPerLine // - characters to show the ascii value
                             + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine)
                .ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = hexChars[(i >> 28) & 0xF];
                line[1] = hexChars[(i >> 24) & 0xF];
                line[2] = hexChars[(i >> 20) & 0xF];
                line[3] = hexChars[(i >> 16) & 0xF];
                line[4] = hexChars[(i >> 12) & 0xF];
                line[5] = hexChars[(i >> 8) & 0xF];
                line[6] = hexChars[(i >> 4) & 0xF];
                line[7] = hexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = hexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = hexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char) b);
                    }

                    hexColumn += 3;
                    charColumn++;
                }

                result.Append(line);
            }

            return result.ToString();
        }
    }
}