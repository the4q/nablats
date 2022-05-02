namespace Nabla.TypeScript.Tool;

public class ArrayCreator<TSource> : TypeDefinitionCreator<TSource>
    where TSource : notnull
{
    public ArrayCreator(TypeFactory<TSource> factory) : base(factory)
    {
    }

    public override TypeBase CreateReference(TSource source, IMetaProvider<TSource> meta, object? state)
    {
        var info = (CollectionInfo<TSource>)state!;

        if (Descriptor.IsRequired(source) || Descriptor.IsTypeDefinition(source))
        {
            return TS.CreateReference(Factory.CreateType(Descriptor.GetTypeDefinition(source)),
                new TypeBase[] {
                        Factory.CreateReference(info.ElementType, meta)
                });
        }

        var property = meta.GetProperty();

        if (property != null)
        {
            meta = info.IsArray ?
                Descriptor.CreateArrayElementMetaProvider(property) :
                Descriptor.CreateGenericArgumentMetaProvider(property, 0);
        }
        else
            meta = new DummyMetaProvider<TSource>(source);
         
        return TS.Array(Factory.CreateReference(info.ElementType, meta));
    }

    public override HitTestResult HitTest(TSource source)
    {
        var info = Descriptor.GetCollectionInfo(source);
        return new(info != null, info);
    }

    protected override TypeBase CreateDefinitionCore(TSource source, object? state)
    {
        var info = (CollectionInfo<TSource>)state!;

        return TS.Alias(Descriptor.ResolveTypeName(source),
            TS.Array(Factory.CreateReference(info.ElementType, new DummyMetaProvider<TSource>(source))));
    }
}
