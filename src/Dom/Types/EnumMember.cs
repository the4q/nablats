namespace Nabla.TypeScript;

public sealed class EnumMember : NamedDomNode
{
    readonly long? _numberValue;
    readonly string? _stringValue;

    public EnumMember(string name)
        : base(name)
    {

    }

    public EnumMember(string name, long value)
        : this(name)
    {
        _numberValue = value;
    }

    public EnumMember(string name, string value)
        : this(name)
    {
        _stringValue = value ?? throw new ArgumentNullException(nameof(value));
    }

    public object? Value => (object?)_numberValue ?? _stringValue;

    public override void Write(TypeWriter writer)
    {
        base.Write(writer);

        if (_numberValue != null || _stringValue != null)
        {
            writer.WriteSpace().Write('=').WriteSpace();

            if (_numberValue != null)
                writer.Write(_numberValue.ToString()!);
            else
                writer.WriteLiteral(_stringValue!);
        }
    }
}