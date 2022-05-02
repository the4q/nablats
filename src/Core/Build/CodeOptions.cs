namespace Nabla.TypeScript.Tool;

public class CodeOptions
{
    public DateHandling DateHandling { get; set; }

    public EnumHandling EnumHandling { get; set; }

    public PropertyNamingPolicy NamingPolicy { get; set; }

    public bool UseNamespaces { get; set; }

    public bool DisableNullable { get; set; }
}
