using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabla.TypeScript.Tool
{
    internal static class UtilityFactory
    {
        public const string ModuleName = "NablatsUtilities";

        internal static readonly object Token = new();

        public static TypeBase OrNull()
        {
            return TS.Alias(nameof(OrNull), TS.Union(TS.Parameter("Type").Reference(), TS.Native.Null()));
        }
    }
}
