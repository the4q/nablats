using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Nabla.TypeScript;

public abstract class DomNode
{
    private DomNode? _parent;
    private List<DomNode>? _children;

    public DomNode()
    {
    }

    public string? Document { get; set; }

    public DomNode? Parent => _parent;

    /// <summary>
    /// Gets or sets anything you want to attach to this node.
    /// </summary>
    public object? UserState { get; set; }

    public virtual TypeFile? DeclaringFile => _parent?.DeclaringFile;

    protected virtual void OnParentChanged(DomNode? newValue, DomNode? oldValue)
    {

    }

    private DomNode? SetParent(DomNode? parent)
    {
        if (parent == this)
            throw new InvalidOperationException("Cannot set parent to self.");

        var old = _parent;

        if (parent != old)
        {
            _parent = parent;
            OnParentChanged(parent, old);
        }

        return old;
    }

    internal protected T Attach<T>(T child)
        where T : DomNode
    {
        child.SetParent(this);

        if (_children == null)
            _children = new();

        _children.Add(child);

        OnAttach(child);

        return child;
    }

    protected virtual void OnAttach(DomNode child)
    {
    }

    internal protected void Attach<T>(params T[] nodes)
        where T : DomNode
    {
        Attach((IEnumerable<DomNode>)nodes);
    }

    internal protected void Attach(IEnumerable<DomNode> nodes)
    {
        foreach (var child in nodes)
            Attach(child);

    }

    internal protected void Deattach(DomNode node)
    {
        if (node._parent != this)
            throw new InvalidOperationException("Invalid child node, does not belong to this instance.");

        node.SetParent(null);

        if (_children != null)
        {
            _children.Remove(node);
            OnDeattach(node);
        }
    }

    protected virtual void OnDeattach(DomNode node)
    {

    }

    internal protected void Deattach<T>(params T[] nodes)
        where T : DomNode
    {
        Deattach((IEnumerable<DomNode>)nodes);
    }

    internal protected void Deattach(IEnumerable<DomNode> nodes)
    {
        foreach (var node in nodes)
            Deattach(node);
    }

    /// <summary>
    /// Deattaches target from current instance and attaches with a new value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <param name="value"></param>
    protected void Replace<T>([NotNullIfNotNull("value")]ref T? target, T? value)
        where T: DomNode
    {
        if (target != value)
        {
            if (target != null)
                Deattach(target);

            target = value;

            if (target != null)
                Attach(target);
        }

    }

    public abstract void Write(TypeWriter writer);

    private static readonly TypeWriterOptions _toStringOptions = new();

    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(GetType().Name).Append(": ");
        using var writer = new TypeWriter(new StringWriter(builder), _toStringOptions, true);
        Write(writer);
        return builder.ToString();
    }

    public IList<DomNode> GetChildren()
    {
        return (IList<DomNode>?)_children?.AsReadOnly() ?? Array.Empty<DomNode>();
    }

    public void WalkDescendents(Action<DomNode> action, bool includeSelf = false)
    {
        IList<DomNode>? starts;

        if (includeSelf) starts = new DomNode[] { this };
        else starts = _children;

        if (starts != null)
            TS.WalkDecendents(starts, action);
    }
}
