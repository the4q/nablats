namespace Nabla.TypeScript;

public class GenericReference : TypeReference
{
    private readonly DomNodeCollection<TypeBase> _arguments;

    public GenericReference(GenericType target, ICollection<TypeBase> arguments)
        : base(target)
    {
        int count = arguments.Count;

        if (count != target.GenericParameters.Count)
            throw new ArgumentException("Parameter count mismatch.");

        _arguments = new(this, arguments);
    }

    public new GenericType Target => (GenericType)base.Target;

    public IList<TypeBase> Arguments
    {
        get => _arguments;
    }

    public override void Write(TypeWriter writer)
    {
        base.Write(writer);

        if (_arguments.Any())
            writer.WriteList(_arguments, WriteListOptions.GenericParameters);
    }
}
