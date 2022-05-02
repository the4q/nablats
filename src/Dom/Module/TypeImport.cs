namespace Nabla.TypeScript;

public sealed class TypeImport : Reference
{
    public TypeImport(TypeFile module)
        : base(module)
    {
        Members = new(this);
    }

    public new TypeFile Target => (TypeFile)base.Target;

    public TypeImportAll? All { get; private set; }

    public DomNodeCollection<TypeImportItem> Members { get; }

    public override void Write(TypeWriter writer)
    {
        if (Members.TryAppend(All).Any())
        {
            writer.Write("import")
                .WriteSpace()
                .Write("type")
                .WriteSpace();

            if (All != null)
                All.Write(writer);
            else
                writer.WriteList(Members, WriteListOptions.ImportMembers);

            writer.WriteSpace()
                .Write("from")
                .WriteSpace()
                .WriteLiteral(Target.GetPath(DeclaringFile!, false))
                .WriteSemicolon();

        }
    }

    public static TypeImport ImportAll(TypeFile file, string alias)
    {
        TypeImport import = new(file);
        import.All = new(file, alias);
        import.Members.SetReadOnly();

        return import;
    }

}
