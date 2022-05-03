using System.Diagnostics.CodeAnalysis;

namespace Nabla.TypeScript;

/// <summary>
/// Represent a base class all reference classes.
/// </summary>
public abstract class Reference : DomNode
{
    DomNode? _target;

    protected Reference(DomNode? target)
    {
        // Do NOT attach target 
        _target = ValidateTargetType(target);
    }

    [return: NotNullIfNotNull("target")]
    private static DomNode? ValidateTargetType(DomNode? target)
    {
        if (target is null)
        {
            return null;
        }

        if (target is not INamedNode)
            throw new ArgumentException("Requires a named node.");

        if (target is ReferenceType)
            throw new ArgumentException("Reference to a ReferenceType is not allowed.");

        return target;
    }

    public DomNode Target
    {
        get
        {
            return _target ?? throw new InvalidOperationException("The target has not been set yet.");
        }
        protected set
        {
            _target = ValidateTargetType(value);
        }
    }

    public string TargetName => ((INamedNode)Target).Name;

    public override DomNodeKind Kind => DomNodeKind.NodeReference;

    public ReferenceType CreateType() => new(this);

}
