namespace Nabla.TypeScript.Tool;

public class PrimitiveCreator<TSource> : TypeCreator<TSource>
    where TSource : notnull
{
    public PrimitiveCreator(TypeFactory<TSource> factory) : base(factory)
    {
    }

    public override TypeBase CreateReference(TSource source, IMetaProvider<TSource> meta, object? state)
    {
        TypeScriptPrimitive primitive = Descriptor.GetPrimitive(source, meta)!.Value;

        if (primitive == TypeScriptPrimitive.Date)
        {
            var handling = Descriptor.GetDateHandling(meta) ?? Factory.Options.DateHandling;

            primitive = handling switch
            {
                DateHandling.Number => TypeScriptPrimitive.Number,
                DateHandling.Date => TypeScriptPrimitive.Date,
                _ => TypeScriptPrimitive.String
            };

        }

        return TS.Native.Primitive(primitive);
    }

    public override HitTestResult HitTest(TSource source)
    {
        return new(Descriptor.IsPrimitive(source));
    }
}
