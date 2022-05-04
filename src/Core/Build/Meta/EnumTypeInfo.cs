namespace Nabla.TypeScript.Tool;

public class EnumTypeInfo
{
    public EnumTypeInfo(IDictionary<string, long> members, EnumHandling? handling, PropertyNamingPolicy? namingPolicy)
    {
        Members = members;
        Handling = handling;
        NamingPolicy = namingPolicy;
    }

    public IDictionary<string, long> Members { get; }

    public EnumHandling? Handling { get; set; }

    public PropertyNamingPolicy? NamingPolicy { get; set; }
}