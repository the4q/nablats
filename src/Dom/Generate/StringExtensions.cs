using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        [return: NotNullIfNotNull("s")]
        public static string? Capitalize(this string? s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var array = s.ToCharArray();

                array[0] = char.ToUpper(array[0]);

                return new(array);
            }

            return s;
        }

        public static string ToTypeName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            StringBuilder builder = new(name);
            int length = builder.Length;
            bool cap = true;

            for (int i = 0; i < length; i++)
            {
                var ch = builder[i];

                if (!char.IsLetterOrDigit(ch))
                {
                    builder.Remove(i, 1);
                    length--;
                    i--;
                    cap = true;
                }
                else if (cap)
                {
                    builder[i] = char.ToUpper(builder[i]);
                    cap = false;
                }
            }

            if (builder.Length == 0)
                throw new ArgumentException("Invalid type name: " + name);


            return builder.ToString();
        }
    }
}
