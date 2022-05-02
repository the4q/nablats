using System.Collections.ObjectModel;

namespace Nabla.TypeScript;

public class DomNodeCollection<TChild> : Collection<TChild>, ICollection<TChild>
    where TChild : DomNode
{
    private readonly DomNode _node;

    private bool _isReadOnly;

    public DomNodeCollection(DomNode node)
    {
        _node = node;
    }

    public DomNodeCollection(DomNode node, IEnumerable<TChild> children)
        : this(node)
    {
        AddRange(children);
        SetReadOnly();
    }

    bool ICollection<TChild>.IsReadOnly => _isReadOnly;

    protected DomNode Node => _node;

    internal void SetReadOnly() => _isReadOnly = true;

    private void ThrowIfReadOnly()
    {
        if (_isReadOnly)
            throw new InvalidOperationException("This collection is currently read-only.");
    }

    protected virtual void Attach(TChild child)
    {
        ThrowIfReadOnly();

        if (child.Parent != null)
        {
            if (!ReferenceEquals(child.Parent, _node))
                throw new InvalidOperationException($"Child node {child} belongs to another collection of {child.Parent}.");
            else
                return;
        }


        _node.Attach(child);
    }

    protected virtual void Deattach(TChild child)
    {
        ThrowIfReadOnly();
        
        if (child.Parent != null && child.Parent != _node)
            throw new InvalidOperationException($"Child node {child} belongs to another collection of {child.Parent}.");

        _node.Deattach(child);
    }

    protected override void ClearItems()
    {
        foreach (var node in this)
            Deattach(node);

        base.ClearItems();

    }

    protected override void InsertItem(int index, TChild item)
    {
        Attach(item);
        base.InsertItem(index, item);
    }

    protected override void RemoveItem(int index)
    {
        Deattach(this[index]);
        base.RemoveItem(index);
    }

    protected override void SetItem(int index, TChild item)
    {
        Deattach(this[index]);
        Attach(item);
        base.SetItem(index, item);
    }

    public void AddRange(IEnumerable<TChild> children)
    {
        ThrowIfReadOnly();

        foreach (var child in children)
            Add(child);
    }

    public void AddRange(params TChild[] children)
        => AddRange((IEnumerable<TChild>)children);
}
