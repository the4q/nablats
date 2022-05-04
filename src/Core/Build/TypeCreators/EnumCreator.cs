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

        if (info != null && info.Handling == null)
            info.Handling = Factory.Options.EnumHandling;

        return new(info != null && info.Handling != EnumHandling.Number, info);
    }

    protected override TypeBase CreateDefinitionCore(TSource source, object? state)
    {
        EnumTypeInfo info = (EnumTypeInfo)state!;
        TypeBase type;
        var handling = info.Handling!.Value;
        var name = Descriptor.ResolveTypeName(source);

        if (handling == EnumHandling.Number)
            throw new InvalidOperationException("Cannot create definition for enum as number type.");

        var members = info.Members.Select(x => (Factory.ResolvePropertyName(x.Key, info.NamingPolicy), x.Value));

        if (handling == EnumHandling.Object || handling == EnumHandling.Const)
        {
            type = TS.Enum(name, members.ToArray()).IsConst(handling == EnumHandling.Const);
        }
        else if (handling == EnumHandling.Union)
        {
            type = TS.Alias(name, TS.Literal(members.Select(x => x.Item1)));
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
