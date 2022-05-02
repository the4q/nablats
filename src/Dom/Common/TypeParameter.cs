namespace Nabla.TypeScript;

public class TypeParameter : NamedDomNode
{
    private readonly Func<TypeBase?> _constraint;
    private readonly Func<TypeBase?> _default;

    public TypeParameter(string name, TypeBase? constraint = null, TypeBase? @default = null) : base(name)
    {
        if (constraint != null)
            Attach(constraint);

        if (@default != null)
            Attach(@default);

        _constraint = () => constraint;
        _default = () => @default;
    }

    private TypeParameter(TypeParameter original)
        : base(original.Name)
    {
        _constraint = () => original.Constraint;
        _default = () => original.Default;
    }

    public TypeBase? Constraint { get => _constraint(); }

    public TypeBase? Default { get => _default(); }

    //public ParameterReference CreateReference() => new(this);

    public TypeParameter CreateProxy(bool forced)
    {
        if (!forced)
            forced = Parent != null;

        if (forced)
            return new(this);
        else
            return this;
    }

    public override void Write(TypeWriter writer)
    {
        base.Write(writer);
        var type = Constraint;

        if (type != null)
        {
            writer.WriteSpace().Write("extends").WriteSpace().WriteNode(type);
        }

        type = Default;
        if (type != null)
            writer.WriteSpace().Write("=").WriteSpace().WriteNode(type);
    }
}
