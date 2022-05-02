namespace Nabla.TypeScript;

public sealed class EnumType : TypeBase, INamedType
{
    private readonly NamedDomNodeCollection<EnumMember> _members;

    public EnumType(string name, IEnumerable<EnumMember> members)
    {
        _members = new(this, members);
        Name = name;
    }

    public string Name { get; }

    public bool IsConst { get; set; }

    public override void Write(TypeWriter writer)
    {
        if (IsConst)
            writer.Write("const").WriteSpace();

        writer
            .Write("enum")
            .WriteSpace()
            .Write(Name)
            .WriteSpace()
            .BeginBlock()
            .WriteList(_members, WriteListOptions.BlockBody)
            .EndBlock();
    }
}
