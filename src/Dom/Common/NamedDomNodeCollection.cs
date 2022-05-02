using System.Diagnostics.CodeAnalysis;

namespace Nabla.TypeScript;

public sealed class NamedDomNodeCollection<TChild> : DomNodeCollection<TChild>
    where TChild : DomNode, INamedNode
{
    readonly Dictionary<string, TChild> _lookup;

    public NamedDomNodeCollection(DomNode node) : base(node)
    {
        _lookup = new Dictionary<string, TChild>();
    }

    public NamedDomNodeCollection(DomNode node, IEnumerable<TChild> children)
        : this(node)
    {
        AddRange(children);
        SetReadOnly();
    }

    protected override void Attach(TChild child)
    {
        if (_lookup.ContainsKey(child.Name))
            throw new InvalidOperationException($"{Node} already contains a named node \"{child.Name}\"");

        _lookup[child.Name] = child;
        base.Attach(child);
    }

    protected override void Deattach(TChild child)
    {
        base.Deattach(child);
        _lookup.Remove(child.Name);
    }

    public ICollection<string> Names => _lookup.Keys;

    public TChild this[string name]
    {
        get => _lookup[name];
    }

    public bool TryGetNode(string name, [NotNullWhen(true)]out TChild? node)
    {
        return _lookup.TryGetValue(name, out node);
    }

    public bool ContainsName(string name)
        => _lookup.ContainsKey(name);
}