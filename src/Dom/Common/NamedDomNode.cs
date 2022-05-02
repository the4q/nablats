namespace Nabla.TypeScript;

public abstract class NamedDomNode : DomNode, INamedNode
{
    public NamedDomNode(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override void Write(TypeWriter writer)
    {
        writer.Write(Name);
    }
}
