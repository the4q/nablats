namespace Nabla.TypeScript.Tool;

internal static class AttributeTargetSets
{
    public const AttributeTargets TypeOnly = AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Enum;

    public const AttributeTargets AssemblyOnly = AttributeTargets.Assembly;

    public const AttributeTargets TypeOrAssembly = AssemblyOnly | TypeOnly;

    public const AttributeTargets PropertyOnly = AttributeTargets.Property | AttributeTargets.Field;

    public const AttributeTargets All = TypeOrAssembly | PropertyOnly;

    public const AttributeTargets ExceptAssembly = All & ~AssemblyOnly;
}
