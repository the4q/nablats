using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabla.TypeScript.Tool;

[AttributeUsage(AttributeTargetSets.ExceptAssembly, AllowMultiple = false, Inherited = false)]
public class TsIgnoreAttribute : Attribute
{

}