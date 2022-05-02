namespace Nabla.TypeScript;

public class TypeReference : Reference
{

    public TypeReference(TypeBase target)
        : base(target)
    {
    }

    public new TypeBase Target => (TypeBase)base.Target;

    public string? GetAlias()
    {
        if (Parent is IAliasProvider aliasProvider)
            return aliasProvider.Alias;
        else
            return null;
    }

    public override void Write(TypeWriter writer)
    {
        writer.Write(GetAlias() ?? Target.GetFullName());
    }
}
