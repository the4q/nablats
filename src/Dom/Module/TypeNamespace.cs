namespace Nabla.TypeScript;

public class TypeNamespace : NamedDomNode, IExportable
{
    public TypeNamespace(string name)
        : base(name)
    {
        Declarations = new(this);
    }

    public NamedDomNodeCollection<TypeDeclaration> Declarations { get; }

    public bool IsLocal { get; internal set; } = true;

    public override void Write(TypeWriter writer)
    {
        if (!IsLocal)
            writer.Write("export").WriteSpace();

        writer.Write("namespace").WriteSpace().Write(Name).WriteSpace();
        writer.BeginBlock().WriteList(Declarations, WriteListOptions.BlockBetween).EndBlock();
    }
}