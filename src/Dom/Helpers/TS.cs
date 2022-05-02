using System.Diagnostics.CodeAnalysis;

namespace Nabla.TypeScript;

/// <summary>
/// Set of factory/helper methods for manipulating TypeScript types.
/// </summary>
public static class TS
{

    public static NativeTypeManager Native { get; } = new();

    /// <summary>
    /// Finds all <see cref="TypeParameter"/>s referenced by certain types.
    /// </summary>
    /// <param name="types">Types to search.</param>
    /// <returns>Returns a collection contains all found parameters.</returns>
    internal static ICollection<TypeParameter> FindParameters(params TypeBase?[] types)
    {
        return FindParameters(types.Where(x => x != null)!);
    }

    /// <summary>
    /// Finds all <see cref="TypeParameter"/>s referenced by certain types.
    /// </summary>
    /// <param name="types">Types to search.</param>
    /// <returns>Returns a collection contains all found parameters.</returns>
    internal static ICollection<TypeParameter> FindParameters(IEnumerable<TypeBase> types)
    {
        HashSet<TypeParameter> parameters = new();

        WalkDecendents(types, node =>
        {
            if (node is ParameterReference pref)
                parameters.Add(pref.Target);
            if (node is TypeParameter param)
                parameters.Add(param);

        });

        return parameters;
    }

    internal static void WalkDecendents(IEnumerable<DomNode> types, Action<DomNode> callback)
        => WalkDecendents(types, (node, _) => callback(node));

    internal static void WalkDecendents(IEnumerable<DomNode> types, Action<DomNode, int> callback)
    {
        HashSet<DomNode> walked = new();

        WalkNested(types, callback, walked, 0);
    }

    private static void WalkNested(IEnumerable<DomNode> types, Action<DomNode, int> callback, HashSet<DomNode> walked, int depth)
    {
        foreach (var type in types)
        {
            if (walked.Contains(type))
                continue;

            walked.Add(type);

            callback(type, depth);

            WalkNested(type.GetChildren(), callback, walked, depth + 1);
        }
    }

    public static ReferenceType CreateReference(TypeBase type, IList<TypeBase?>? arguments)
    {
        if (type is GenericType generic)
        {
            
            TypeBase[] args = new TypeBase[generic.GenericParameters.Count];

            for (int i = 0; i < args.Length; i++)
            {
                TypeBase? arg;

                if (arguments != null && i < arguments.Count)
                {
                    arg = arguments[i];
                }
                else
                    arg = null;

                args[i] = arg ?? generic.GenericParameters[i].Reference();
            }

            return new GenericReference(generic, args).CreateType();
        }
        else
            return new TypeReference(type).CreateType();

    }

    /// <summary>
    /// Creates a TypeScript mapped object, like { [ P in K ]: T }.
    /// </summary>
    /// <param name="propertyProvider">A type which provides keys of object.</param>
    /// <param name="proeprtyType">Property type</param>
    /// <param name="variableName">Property variable name.</param>
    /// <returns></returns>
    public static ObjectType MappedObject(TypeBase propertyProvider, TypeBase proeprtyType, string variableName = "P")
    {
        PropertyIterator iterator = new(variableName, propertyProvider);
        TypeProperty[] properties = new[]
        {
            new TypeProperty(iterator, proeprtyType),
        };

        var parameters = FindParameters(propertyProvider, proeprtyType).ToArray();
        return new(properties, parameters);
    }

    


    public static ArrayType Array(TypeBase elementType, int levels = 1)
    {
        if (levels < 1)
            throw new ArgumentOutOfRangeException(nameof(levels));

        if (levels > 1)
            return new(Array(elementType, levels - 1));
        else
            return new(elementType);
    }

    public static LiteralType Literal(string value)
        => new(value);

    public static UnionType Literal(params string[] values)
        => Literal((IEnumerable<string>)values);

    public static UnionType Literal(IEnumerable<string> values)
        => new(values.Select(x => Literal(x)), null);

    public static AliasType Alias(string name, TypeBase type)
        => new(name, type);

    public static UnionType Union(params TypeBase[] types)
        => new(types, FindParameters(types));

    public static InterfaceType Interface(string name, ReferenceType? baseType, params TypeProperty[] properties)
    {
        return new(name, properties, baseType);
    }

    public static TypeProperty Property(string name, TypeBase type, bool isOptional = false, bool isReadonly = false)
        => new(name, type) { IsOptional = isOptional, IsReadOnly = isReadonly };

    public static TypeParameter Parameter(string name, TypeBase? constraint = null, TypeBase? @default = null)
        => new(name, constraint, @default);

    public static TupleType Tuple(params TypeBase[] types)
        => Tuple((IEnumerable<TypeBase>)types);

    public static TupleType Tuple(IEnumerable<TypeBase> types)
        => new(types);

    public static EnumType Enum(string name, IDictionary<string, long> members)
    {
        return new(name, members.Select(x => new EnumMember(x.Key, x.Value)));
    }

    public static EnumType Enum(string name, params (string, long)[] members)
    {
        return new(name, members.Select(x => new EnumMember(x.Item1, x.Item2)));
    }

    public static EnumType Enum(string name, IDictionary<string, string> members)
    {
        return new(name, members.Select(x => new EnumMember(x.Key, x.Value)));
    }

    public static EnumType Enum(string name, params (string, string)[] members)
    {
        return new(name, members.Select(x => new EnumMember(x.Item1, x.Item2)));
    }

    public static EnumType Enum(string name, params string[] members)
    {
        return new(name, members.Select(x => new EnumMember(x)));
    }

    public static EnumType Enum(Type clrEnumType, string? name = null, Func<string, string>? translateMemberName = null)
    {
        if (!clrEnumType.IsEnum)
            throw new ArgumentException($"{clrEnumType.FullName} is not a Enum type.");

        var names = System.Enum.GetNames(clrEnumType);
        var values = System.Enum.GetValues(clrEnumType);

        return new(name ?? clrEnumType.Name,
            names.Select((x, i) => new EnumMember(translateMemberName?.Invoke(x) ?? x, Convert.ToInt64(values.GetValue(i)))));
    }

    public static EnumType IsConst(this EnumType enumType, bool isConst = true)
    {
        enumType.IsConst = isConst;
        return enumType;
    }

    public static JsDoc? JsDoc(TypeBase type)
    {
        var @params = FindParameters(type);
        var remarks = type.Document;

        if (!string.IsNullOrEmpty(remarks) || @params.Where(x => !string.IsNullOrEmpty(x.Document)).Any())
        {
            JsDoc doc = new() { Remarks = remarks };

            foreach (var param in @params)
            {
                if (param.Document != null)
                    doc.Parameters.Add(param.Name, param.Document);
            }

            return doc;
        }

        return null;
    }

    public static TypeNamespace Namespace(string name)
    {
        return new(name);
    }

    public static string PopulateFullName(string typeName, string? namespc)
    {
        if (namespc != null)
            return $"{namespc}.{typeName}";
        else
            return typeName;
    }

    public static bool TryParseFullName(string fullName, out string typeName, [NotNullWhen(true)]out string? namespc)
    {
        int p = fullName.LastIndexOf('.');

        if (p >= 0)
        {
            typeName = fullName[(p + 1)..];
            
            if (p > 0)
            {
                namespc = fullName[..p];
            }
            else
                namespc = null;
        }
        else
        {
            typeName = fullName;
            namespc = null;
        }

        return namespc != null;
    }
}
