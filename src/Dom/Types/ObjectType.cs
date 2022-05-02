namespace Nabla.TypeScript;

public class ObjectType : GenericType
{
    public ObjectType(IEnumerable<TypeProperty> properties, IEnumerable<TypeParameter>? parameters)
        : base(parameters)
    {
        Properties = new(this, properties);
    }

    public NamedDomNodeCollection<TypeProperty> Properties { get; }

    public override void Write(TypeWriter writer)
    {
        writer.BeginBlock().WriteList(Properties, WriteListOptions.BlockBody).EndBlock();
    }

}
