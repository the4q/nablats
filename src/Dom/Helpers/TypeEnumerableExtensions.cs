using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    internal static class TypeEnumerableExtensions
    {
        public static IEnumerable<T> TryAppend<T>(this IEnumerable<T> source, T? element)
        {
            if (element is not null)
                return source.Append(element);

            return source;
        }
    }
}
