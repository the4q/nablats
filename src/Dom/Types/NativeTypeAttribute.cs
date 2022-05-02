namespace Nabla.TypeScript;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
internal class NativeTypeAttribute : Attribute
{
    public NativeTypeAttribute()
    {

    }

    public NativeTypeAttribute(string name)
    {
        Name = name;
    }

    public string? Name { get; set; }

    public NativeTypeCategory Category { get; set; }
}
