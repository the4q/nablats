namespace Nabla.TypeScript.Tool;

public static class NativeTypeExtensions
{
    public static ReferenceType Primitive(this NativeTypeManager manager, TypeScriptPrimitive primitive)
    {
        string name = primitive.ToString();

        if (primitive < TypeScriptPrimitive.Date)
            name = name.ToLower();

        return manager.Primitive(name);
    }

}
