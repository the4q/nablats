namespace Nabla.TypeScript.Tool;

[AttributeUsage(AttributeTargetSets.TypeOrAssembly, AllowMultiple = false)]
public class TsFileNameAttribute : Attribute
{
    public TsFileNameAttribute()
    {

    }

    public TsFileNameAttribute(string name)
    {
        Name = name;
    }

    public string? Name { get; }

}
