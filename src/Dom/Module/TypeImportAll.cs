namespace Nabla.TypeScript;

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