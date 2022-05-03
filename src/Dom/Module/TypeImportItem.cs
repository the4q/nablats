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
