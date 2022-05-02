using System.Diagnostics;

namespace Nabla.TypeScript.Tool;

public class InterfaceCreator<TSource> : TypeDefinitionCreator<TSource>
    where TSource : notnull
{
    public InterfaceCreator(TypeFactory<TSource> factory) : base(factory)
    {
    }

    public override HitTestResult HitTest(TSource source)
    {
        return new(true);
    }

    protected override TypeBase CreateDefinitionCore(TSource source, object? state)
    {
        string name;
        ReferenceType? baseType;

        if (Descriptor.IsClrTuple(source, out var tupleType, out var tupleArgs))
        {
            name = "Tuple" + tupleArgs.Length;
            source = tupleType;
            baseType = null;
        }
        else
        {
            name = Descriptor.ResolveTypeName(source);

            var baseClrType = Descriptor.GetBaseType(source);

            if (baseClrType != null)
                baseType = (ReferenceType)CreateReference(baseClrType, new DummyMetaProvider<TSource>(source), state);
            else
                baseType = null;
        }

        List<TypeProperty> properties = new();

        foreach (var member in Descriptor.GetProperties(source, false))
        {
            properties.Add(CreateProperty(member));
        }

        return TS.Interface(name, baseType, properties.ToArray());
    }

    private TypeProperty CreateProperty(IPropertyMetaProvider<TSource> member)
    {
        TypeBase type = CreatePropertyType(member);

        return TS.Property(Descriptor.ResolvePropertyName(member), type, isOptional: member.IsNullable, isReadonly: member.IsReadOnly);
    }

    protected TypeBase CreatePropertyType(IPropertyMetaProvider<TSource> member)
    {
        TypeBase type;

        var ova = member.TypeOverride;

        if (ova != null)
        {
            type = Factory.UseTypeOverride(member);
            if (ova.ArrayDepth > 0)
                type = TS.Array(type, ova.ArrayDepth);
        }
        else
        {
            type = Factory.CreateReference(Descriptor.GetUnderlyingSource(member.Source), member);
        }

        return type;
    }

    public override TypeBase CreateReference(TSource source, IMetaProvider<TSource> meta, object? state)
    {
        TSource clrTypeDef;

        if (!Descriptor.IsTypeDefinition(source))
        {
            Debug.Assert(Descriptor.IsGenericType(source));

            clrTypeDef = Descriptor.GetTypeDefinition(source);
        }
        else
        {
            clrTypeDef = source;
        }

        if (Factory.IsSourceCreating(clrTypeDef))
            return Factory.Defer(clrTypeDef, meta);

        if (Descriptor.IsClrTuple(clrTypeDef, out var tupleType, out _))
            clrTypeDef = tupleType;

        TypeBase tsTypeDef = Factory.CreateType(clrTypeDef);

        var genArgs = Descriptor.GetGenericArguments(source);

        return TS.CreateReference(tsTypeDef, genArgs.Select(x => Factory.CreateReference(x, meta)).ToList()!);

    }
}
