using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirikiriTest
{
    public static class StringExtensions
    {

        public static string GetLine(this string text, int lineNumber)
        {
            using (var reader = new StringReader(text))
            {
                string line;
                int currentLineNumber = 0;

                do
                {
                    currentLineNumber += 1;
                    line = reader.ReadLine();
                } while (line != null && currentLineNumber < lineNumber);

                return currentLineNumber == lineNumber ? line : String.Empty;
            }
        }

        public static string GetSegment(this string text, int startLine, int startColumn, int endLine, int endColumn)
        {
            var sb = new StringBuilder();
            using (var reader = new StringReader(text))
            {
                int currentLineNumber = 0;

                do
                {
                    currentLineNumber += 1;
                    string line = reader.ReadLine();
                    if (line == null) break;
                    int startPos = currentLineNumber == startLine ? startColumn : 0;
                    int endPos = currentLineNumber == endLine ? endColumn : 0;
                    sb.Append(line.Substring(startPos, endPos - startPos));
                } while (currentLineNumber < endLine);

                return sb.ToString();
            }
        }


        public static void TryGetPosition(this string input, int position, out int line, out int column)
        {

            line = 1;
            column = 0;
            for (int i = 0; i < position; i++)
            {
                if (i >= input.Length)
                    break;
                if (input[i] == '\n')
                {
                    line++;
                    column = 0;
                }
                column++;
            }
        }

        public static int IndexOfNth(this string str, char c, int n)
        {
            int s = -1;

            for (int i = 0; i < n; i++)
            {
                s = str.IndexOf(c, s + 1);

                if (s == -1) break;
            }

            return s;
        }
    }
}
