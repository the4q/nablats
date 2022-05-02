namespace Nabla.TypeScript.Tool;

public class RecordCreator<TSource> : TypeDefinitionCreator<TSource>
    where TSource : notnull
{
    public RecordCreator(TypeFactory<TSource> factory) : base(factory)
    {
    }

    public override HitTestResult HitTest(TSource source)
    {
        var info = Descriptor.GetDictionaryInfo(source);
        return new(info != null, info);
    }

    private ReferenceType GetKeyPrimitive(TSource key, IMetaProvider<TSource>? meta)
    {
        var pt = Descriptor.GetPrimitive(key, meta);

        if (pt == null)
            throw new CodeException("Non-primitive type is not supported as dictionary key.");

        return TS.Native.Primitive(pt.Value);
    }

    public override TypeBase CreateReference(TSource source, IMetaProvider<TSource> meta, object? state)
    {
        var info = (RecordTypeInfo<TSource>)state!;

        var property = meta.GetProperty();

        if (property != null)
            meta = Descriptor.CreateGenericArgumentMetaProvider(property, 1);

        return TS.Native.Record(GetKeyPrimitive(info.Key, meta), Factory.CreateReference(info.Value, meta));
    }

    protected override TypeBase CreateDefinitionCore(TSource source, object? state)
    {

        var info = (RecordTypeInfo<TSource>)state!;

        if (Descriptor.IsTypeParameter(info.Key))
            throw new CodeException($"Key type must be explicitly specified for dictionary derrived type {Descriptor.Describe(source)}.");


        var genArgs = Descriptor.GetGenericArguments(source);

        if (genArgs.Count != (Descriptor.IsTypeParameter(info.Value) ? 1 : 0))
            throw new CodeException($"Dictionary type {Descriptor.Describe(source)} contains extra generic paramters that is not allowed.");

        TypeBase vt;

        if (Descriptor.IsTypeParameter(info.Value))
        {
            vt = TS.Parameter(Descriptor.GetName(info.Value)).Reference();
        }
        else
        {
            // Currently we are unable to infer nullability info from generic parameter.

            vt = Factory.CreateReference(info.Value, new DummyMetaProvider<TSource>(source));
        }

        var type = TS.Alias(Descriptor.ResolveTypeName(source), TS.Native.Record(GetKeyPrimitive(info.Key, null), vt));

        return type;

    }
}
