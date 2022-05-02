namespace Nabla.TypeScript;

/// <summary>
/// Represent a TypeScript type which contains a reference to another type.
/// </summary>
public sealed class ReferenceType : TypeBase
{
    Reference _reference;

    public ReferenceType(Reference reference)
    {
        _reference = Attach(reference);
    }

    public Reference Reference
    {
        get => _reference;
        set => Replace(ref _reference, value);
    }

    public override void Write(TypeWriter writer)
    {
        Reference.Write(writer);
    }

    public DomNode DeattachTarget()
    {
        if (Parent != null)
            throw new InvalidOperationException("Current node already has a parent, deattach from it then try again.");

        Deattach(Reference);

        return Reference.Target;
    }

    public void Inject(Func<Reference, Reference> func)
    {
        Deattach(_reference);
        Attach(func(_reference));
    }
}