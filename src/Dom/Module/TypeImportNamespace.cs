namespace Nabla.TypeScript;

public sealed class TypeImportNamespace : TypeImportItem
{
    internal TypeImportNamespace(TypeNamespace target, string? alias)
        : base(target, alias)
    {

    }

    public new TypeNamespace Target => (TypeNamespace)base.Target;

    protected override void WriteName(TypeWriter writer)
    {
        var name = Target.Name;
        var p = name.IndexOf('.');

        if (p > 0)
            name = name[0..p];

        writer.Write(name);
    }

    internal override bool IsMatch(TypeBase type)
    {
        return type.GetNamespace() == Target;
    }
}
