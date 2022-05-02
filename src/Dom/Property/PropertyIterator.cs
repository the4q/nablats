namespace Nabla.TypeScript;

internal sealed class PropertyIterator : NamedDomNode
{
    public PropertyIterator(string variableName, TypeBase provider)
        : base("[__iterator__]")
    {
        VariableName = variableName;
        Provider = Attach(provider);
    }

    public string VariableName { get; }

    public TypeBase Provider { get; }

    public override void Write(TypeWriter writer)
    {
        writer
            .Write('[')
            .WriteSpace()
            .Write(VariableName)
            .WriteSpace()
            .Write("in")
            .WriteSpace()
            .WriteNode(Provider)
            .WriteSpace()
            .Write(']');
    }

}
