using System.Diagnostics;

namespace Nabla.TypeScript.Tool;

internal sealed class DeferredReference<TSource> : Reference
    where TSource: notnull
{
    TypeReference? _ref;

    public DeferredReference(TSource source, IMetaProvider<TSource> meta)
        : base(null)
    {
        Source = source;
        Meta = meta;

    }

    public TSource Source { get; }

    public IMetaProvider<TSource> Meta { get; }

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
