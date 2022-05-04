using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabla.TypeScript.Tool
{
    public class TupleCreator<TSource> : InterfaceCreator<TSource>
        where TSource : notnull
    {
        public TupleCreator(TypeFactory<TSource> factory) : base(factory)
        {
        }

        public override HitTestResult HitTest(TSource source)
        {
            return new(Descriptor.IsTypeScriptTuple(source));
        }

        protected override TypeBase CreateDefinitionCore(TSource source, object? state)
        {
            List<TypeBase> types = new();

            foreach (var property in Descriptor.GetProperties(source, true).OrderBy(x => Descriptor.GetTupleOrder(x) ?? 0))
            {
                types.Add(CreatePropertyType(property));
            }

            return TS.Alias(Descriptor.ResolveTypeName(source), new TupleType(types));
        }
    }
}
