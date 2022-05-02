namespace Nabla.TypeScript;

public class ParameterReference : Reference
{
    public ParameterReference(TypeParameter target)
        : base(target)
    {
    }

    public override void Write(TypeWriter writer)
    {
        writer.Write(TargetName);
    }

    public new TypeParameter Target => (TypeParameter)base.Target;
}
