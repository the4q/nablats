namespace Nabla.TypeScript;

public class TupleType : GenericType
{
    readonly DomNodeCollection<TypeBase> _types;

    public TupleType(IEnumerable<TypeBase> types)
        : base(TS.FindParameters(types).CreateProxies())
    {
        _types = new(this, types);
    }

    public override void Write(TypeWriter writer)
    {
        writer.WriteList(_types, WriteListOptions.Array);
    }
}
