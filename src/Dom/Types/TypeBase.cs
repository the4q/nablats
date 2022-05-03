namespace Nabla.TypeScript;

public abstract class TypeBase : DomNode
{
    public override DomNodeKind Kind { get; } = DomNodeKind.TypeDefinition;
}
