namespace Nabla.TypeScript;

internal sealed class ImportProxy : Reference, IAliasProvider
{
    private readonly TypeReference _reference;

    public ImportProxy(TypeImportItem importItem, TypeReference reference)
        : base(importItem.Target)
    {
        ImportItem = importItem;
        _reference = Attach(reference);
    }

    public TypeImportItem ImportItem { get; }

    public string? Alias => ImportItem.Alias;

    public override void Write(TypeWriter writer)
    {
        _reference.Write(writer);
    }
}
