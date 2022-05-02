namespace Nabla.TypeScript.Tool;

public class VariantTypeInfo
{
    public VariantTypeInfo(VariantTypeHandling handling, string? genericParameterName)
    {
        Handling = handling;
        GenericParameterName = genericParameterName;
    }

    public VariantTypeHandling Handling { get; }

    public string? GenericParameterName { get; }
}