namespace Nabla.TypeScript;

public sealed class PlainTextName : NamedDomNode
{
    public PlainTextName(string name)
        : base(name)    
    {
    }

    public override DomNodeKind Kind => DomNodeKind.Identifier;
}
