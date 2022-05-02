namespace Nabla.TypeScript.Tool;

[AttributeUsage(AttributeTargetSets.TypeOnly, AllowMultiple = false, Inherited = false)]
public class TsTypeNameAttribute : Attribute
{
    public TsTypeNameAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
