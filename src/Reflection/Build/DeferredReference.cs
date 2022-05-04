using System.Diagnostics;

namespace Nabla.TypeScript.Tool.Reflection;

internal sealed class DeferredReference : Reference
{
    TypeReference? _ref;

    public DeferredReference(Type clrType, ISite site)
        : base(null)
    {
        ClrType = clrType;
        Site = site;
        
    }

    public Type ClrType { get; }

    public ISite Site { get; }

    public void Solve(ReferenceType refType)
    {
        Debug.Assert(refType.Reference is TypeReference);

        var type = (TypeBase)refType.DeattachTarget();

        Target = type;
        _ref = (TypeReference)refType.Reference;
    }

    public override void Write(TypeWriter writer)
    {
        if (_ref == null)
            throw new InvalidOperationException("Deferred reference has not been solved.");

        _ref.Write(writer);
    }
}
