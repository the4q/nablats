namespace Nabla.TypeScript.Tool;

[AttributeUsage(AttributeTargetSets.All, AllowMultiple = false)]
public class TsDateHandlingAttribute : Attribute
{
    public TsDateHandlingAttribute(DateHandling handling)
    {
        Handling = handling;
    }

    public DateHandling Handling { get; }

}
