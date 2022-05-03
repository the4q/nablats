using System.Diagnostics.CodeAnalysis;

namespace Nabla.TypeScript;

public class TypeProperty : DomNode, INamedNode
{
    private TypeBase _fieldType;

    public TypeProperty(string name, TypeBase type)
        : this(new PlainTextName(name), type)
    {

    }

    public TypeProperty(NamedDomNode identifier, TypeBase type)
    {
        Identifier = Attach(identifier);
        Type = Attach(type);
    }

    public bool IsOptional { get; set; }

    public bool IsReadOnly { get; set; }

    public string Name => Identifier.Name;

    public override DomNodeKind Kind => DomNodeKind.Property;

    public NamedDomNode Identifier { get; }

    public TypeBase Type
    {
        get => _fieldType!;
        [MemberNotNull(nameof(_fieldType))]
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            Replace(ref _fieldType, value);
        }
    }

    public override void Write(TypeWriter writer)
    {
        if (IsReadOnly)
            writer.Write("readonly").WriteSpace();

        Identifier.Write(writer);

        if (IsOptional)
            writer.Write('?');

        writer.Write(':').WriteSpace();

        Type.Write(writer);
    }
}
