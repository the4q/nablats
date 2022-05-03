namespace Nabla.TypeScript;

public sealed class TypeImportType : TypeImportItem
{
    internal TypeImportType(TypeBase target, string? alias)
        : base(target, alias)
    {
    }

    public new TypeBase Target => (TypeBase)base.Target;

    protected override void WriteName(TypeWriter writer)
    {
        writer.Write(TargetName);
    }

    internal override bool IsMatch(TypeBase type)
    {
        return Target == type;
    }
}
