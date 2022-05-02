namespace Nabla.TypeScript;

public class InterfaceType : ObjectType, INamedType
{
    public InterfaceType(string name, IEnumerable<TypeProperty> properties, ReferenceType? baseType = null)
        : base(properties, TS.FindParameters(properties.Select(x => x.Type).TryAppend(baseType)).CreateProxies())
    {
        Name = name;

        if (baseType != null)
            BaseType = Attach(baseType);

    }

    public ReferenceType? BaseType { get; }

    public string Name { get; }

    public override void Write(TypeWriter writer)
    {
        writer
            .Write("interface")
            .WriteSpace()
            .Write(Name)
            .WriteGenericParameters(GenericParameters)
            .WriteSpace();
        
        if (BaseType != null)
            writer.Write("extends").WriteSpace().WriteNode(BaseType);

        writer.WriteSpace();
        base.Write(writer);
    }

}
