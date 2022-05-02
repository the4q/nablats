namespace Nabla.TypeScript;

public class UnionType : GenericType
{
    private readonly DomNodeCollection<TypeBase> _types;

    public UnionType(IEnumerable<TypeBase> types, IEnumerable<TypeParameter>? parameters)
        : base(parameters)
    {
        _types = new(this, types);
    }

    public IList<TypeBase> Types => _types;

    public override void Write(TypeWriter writer)
    {
        writer.WriteList(Types, WriteListOptions.Union);
    }
}
