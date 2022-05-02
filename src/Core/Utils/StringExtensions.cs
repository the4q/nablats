using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    internal static class StringExtensions
    {
        static readonly char[] _lineSeparators = Environment.NewLine.ToCharArray();

        public static string[] ToLines(this string? s, bool removeEmptyLines = false)
        {
            if (s != null)
            {
                StringSplitOptions options =
                    removeEmptyLines ? StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries :
                    StringSplitOptions.None;

                return s.Split(_lineSeparators, options);
            }

            return Array.Empty<string>();
        }
    }
}
