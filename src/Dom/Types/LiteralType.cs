namespace Nabla.TypeScript;

public class LiteralType : TypeBase
{
    public LiteralType(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public override void Write(TypeWriter writer)
    {
        writer.WriteLiteral(Value);
    }
}