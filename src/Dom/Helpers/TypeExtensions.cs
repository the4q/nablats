using System.Diagnostics.CodeAnalysis;

namespace Nabla.TypeScript;

public static class TypeExtensions
{
    public static ReferenceType Reference(this TypeBase type, params TypeBase?[] arguments)
    {
        return TS.CreateReference(type, arguments);
    }

    //public static ReferenceType Reference(this TypeImportItem item, params TypeBase?[] argumetns)
    //{
    //    return TS.CreateReference(item.Target, argumetns);
    //}

    public static ReferenceType Reference(this TypeParameter parameter)
        => new ParameterReference(parameter).CreateType();

    public static IEnumerable<TypeParameter> CreateProxies(this IEnumerable<TypeParameter> parameters, bool forced = false)
       => parameters.Select(x => x.CreateProxy(forced));

    public static bool TryGetName(this TypeBase type, [NotNullWhen(true)] out string? name)
    {
        if (type is INamedType namedType)
        {
            name = namedType.Name;
            return true;
        }

        name = null;
        return false;
    }

    public static string GetName(this TypeBase type)
    {
        if (type.TryGetName(out var name))
            return name;

        throw new InvalidOperationException($"The specified type {type.GetType().FullName} does not have a name.");
    }

    public static string GetFullName(this TypeBase type)
    {
        var name = type.GetName();
        var ns = type.GetNamespace();

        return TS.PopulateFullName(name, ns?.Name);
    }

    public static TypeNamespace? GetNamespace(this TypeBase type)
    {
        DomNode? node = type;

        while (node != null)
        {
            if (node is TypeNamespace ns)
                return ns;

            node = node.Parent;
        }

        return null;
    }

    public static T Document<T>(this T node, string document)
        where T : DomNode
    {
        node.Document = document;
        return node;
    }

}
