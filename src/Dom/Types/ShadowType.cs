namespace Nabla.TypeScript;

public class ShadowType : GenericType, INamedType
{
    public ShadowType(string name)
        : this(name, null)
    {

    }

    public ShadowType(string name, IEnumerable<TypeParameter>? parameters)
        : base(parameters)
    {
        Name = name;
    }

    public ShadowType(string name, int parameterCount, string parameterFormat = "P{0}")
        : this(name, Enumerable.Repeat(parameterFormat, parameterCount).Select((f, i) => TS.Parameter(string.Format(f, i))))
    {

    }

    public string Name { get; }

    public override DomNodeKind Kind => DomNodeKind.Unknown;

    public override void Write(TypeWriter writer)
    {
        // do not output anything about this type
    }

    public override string ToString()
    {
        return $"{GetType().Name}: {Name} from {DeclaringFile?.Name ?? "*UNKNWON*"}";
    }
}