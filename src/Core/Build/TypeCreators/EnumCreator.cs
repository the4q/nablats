namespace Nabla.TypeScript.Tool;

public class EnumCreator<TSource> : TypeDefinitionCreator<TSource>
    where TSource : notnull
{
    public EnumCreator(TypeFactory<TSource> factory) : base(factory)
    {
    }

    public override HitTestResult HitTest(TSource source)
    {
        var info = Descriptor.GetEnumInfo(source);

        return new(info != null && info.Handling != EnumHandling.Number, info);
    }

    protected override TypeBase CreateDefinitionCore(TSource source, object? state)
    {
        EnumTypeInfo info = (EnumTypeInfo)state!;
        TypeBase type;
        var handling = info.Handling;
        var name = Descriptor.ResolveTypeName(source);

        if (handling == EnumHandling.Object || handling == EnumHandling.Const)
        {
            type = TS.Enum(name, info.Members).IsConst(handling == EnumHandling.Const);
        }
        else if (handling == EnumHandling.Union)
        {
            type = TS.Alias(name, TS.Literal(info.Members.Select(x => x.Key)));
        }
        else
            throw new NotImplementedException();

        return type;
    }

    public override TypeBase CreateReference(TSource source, IMetaProvider<TSource> meta, object? state)
    {
        EnumTypeInfo info = (EnumTypeInfo)state!;

        var type = Factory.Define(source, () => CreateDefinitionCore(source, state));

        return TS.CreateReference(type, null);
    }
}
