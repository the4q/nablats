namespace Nabla.TypeScript.Tool;

public class EnumTypeInfo
{
    public EnumTypeInfo(IDictionary<string, long> members, EnumHandling handling)
    {
        Members = members;
        Handling = handling;
    }

    public IDictionary<string, long> Members { get; }

    public EnumHandling Handling { get; }
}