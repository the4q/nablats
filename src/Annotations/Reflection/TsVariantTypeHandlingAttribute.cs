namespace Nabla.TypeScript.Tool;

[AttributeUsage(AttributeTargetSets.PropertyOnly, AllowMultiple = false)]
public class TsVariantTypeHandlingAttribute : Attribute
{
    public TsVariantTypeHandlingAttribute(VariantTypeHandling handling)
    {
        Handling = handling;
    }

    public TsVariantTypeHandlingAttribute(string genericParameterName)
    {
        GenericParameterName = genericParameterName;
        Handling = VariantTypeHandling.AsGenericParameter;
    }

    public VariantTypeHandling Handling { get; }

    public string? GenericParameterName { get; }
}