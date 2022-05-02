namespace Nabla.TypeScript;

public sealed class NativeType : ShadowType, INamedType
{
    internal NativeType(string name, IEnumerable<TypeParameter> parameters, NativeTypeCategory category)
        : base(name, parameters)
    {
        Category = category;
    }

    public NativeTypeCategory Category { get; }

    public bool IsPrimitive => Category != NativeTypeCategory.TypeScriptUtility;
}

