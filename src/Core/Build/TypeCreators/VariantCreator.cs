namespace Nabla.TypeScript.Tool;

public class VariantCreator<TSource> : TypeCreator<TSource>
    where TSource: notnull
{
    public VariantCreator(TypeFactory<TSource> factory) : base(factory)
    {
    }

    public override TypeBase CreateReference(TSource source, IMetaProvider<TSource> meta, object? state)
    {
        VariantTypeInfo info = Descriptor.GetVariantInfo(source, meta);
        var member = meta as IPropertyMetaProvider<TSource>;
        TypeBase type;

        if (info.Handling == VariantTypeHandling.AsGenericParameter)
        {
            var typeName = info.GenericParameterName ?? member?.Name + "Type";

            type = TS.Parameter(typeName).Reference();

        }
        else
            type = TS.Native.Unknown();

        return type;
    }

    public override HitTestResult HitTest(TSource source)
    {
        return new(Descriptor.IsVariant(source));
    }
}