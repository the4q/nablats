namespace Nabla.TypeScript.Tool;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Enum, AllowMultiple = false)]
public class TsEnumHandlingAttribute : Attribute
{
    PropertyNamingPolicy? _namingPolicy;

    public TsEnumHandlingAttribute(EnumHandling handling)
    {
        Handling = handling;
    }

    public EnumHandling Handling { get; }

    public PropertyNamingPolicy NamingPolicy
    {
        get => _namingPolicy ?? default;
        set => _namingPolicy = value;
    }

    public PropertyNamingPolicy? GetNamingPolicy() => _namingPolicy;
}
