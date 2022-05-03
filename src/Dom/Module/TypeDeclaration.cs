namespace Nabla.TypeScript;

public sealed class TypeDeclaration : NamedDomNode, IExportable
{
    public TypeDeclaration(INamedType definition)
        : this(definition.Name, (TypeBase)definition)
    {

    }

    internal TypeDeclaration(string name, TypeBase definition)
        : base(name)
    {
        Type = Attach(definition);
    }

    public TypeBase Type { get; }

    public bool IsLocal { get; init; }

    public bool IsDeclare { get; init; }

    public override DomNodeKind Kind => DomNodeKind.TypeDeclaration;

    public override void Write(TypeWriter writer)
    {
        TS.JsDoc(Type)?.Write(writer);

        if (!IsLocal)
            writer.Write("export").WriteSpace();

        if (IsDeclare)
            writer.Write("declare").WriteSpace();

        Type.Write(writer);
    }

}
