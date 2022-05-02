namespace Nabla.TypeScript;

public abstract class TypeImportItem : Reference, IAliasProvider
{
    protected TypeImportItem(DomNode target, string? alias)
        : base(target)
    {
        Alias = alias;
    }

    public string? Alias { get; internal set; }

    public sealed override void Write(TypeWriter writer)
    {
        WriteName(writer);

        if (Alias != null)
        {
            writer.WriteSpace().Write("as").WriteSpace().Write(Alias);
        }
    }

    protected abstract void WriteName(TypeWriter writer);

    internal abstract bool IsMatch(TypeBase type);
}

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

public class TypeImportAll : TypeImportItem
{
    internal TypeImportAll(TypeFile file, string alias)
        : base(file, alias ?? throw new ArgumentNullException(nameof(alias)))
    {

    }

    public new string Alias => base.Alias!;

    protected override void WriteName(TypeWriter writer)
    {
        writer.Write('*');
    }

    internal override bool IsMatch(TypeBase type)
    {
        return type.DeclaringFile == Target;
    }
}