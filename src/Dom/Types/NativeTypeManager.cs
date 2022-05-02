using System.Reflection;

namespace Nabla.TypeScript;

public sealed class NativeTypeManager : TypeFile
{


    internal NativeTypeManager()
        : base("TypeScriptNative")
    {
        Declarations.AddRange(CreateTypes());
        Declarations.SetReadOnly();
    }

    public override ModuleReferenceMode ReferenceMode => ModuleReferenceMode.Implicit;

    //private static TypeDeclaration CreateRecordType()
    //{
    //    //var keyBaseType = new UnionType(NativeType.References(TypeScriptNativeKind.String, TypeScriptNativeKind.Number, TypeScriptNativeKind.Symbol), null);
    //    //var keyParam = new TypeParameter("K", keyBaseType, null);
    //    //var valueParam = new TypeParameter("T", null, null);

    //    //var def = TS.MappedObject(
    //    //    keyParam.Reference(),
    //    //    valueParam.Reference()
    //    //);

    //    return new(new AliasType("Record", TS.Tuple(TS.Parameter("Keys").Reference(), TS.Parameter("Type").Reference())));
    //}

    private static IEnumerable<TypeDeclaration> CreateTypes()
    {
        foreach (var method in typeof(NativeTypeManager).GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            var attr = method.GetCustomAttribute<NativeTypeAttribute>();

            if (attr != null)
            {
                var methodParams = method.GetParameters();
                var typeParams = new List<TypeParameter>(methodParams.Length);
                var tname = attr.Name ?? method.Name;

                for (int i = 0; i < methodParams.Length; i++)
                {
                    string pname = methodParams[i].Name!;

                    pname = char.ToUpper(pname[0]) + pname[1..];

                    typeParams.Add(TS.Parameter(pname));
                }

                yield return new(tname, new NativeType(tname, typeParams, attr.Category));
            }
        }
    }

    private NativeType GetType(string name)
    {
        return (NativeType)Declarations[name].Type;

    }

    private ShadowType GetType(MethodBase? method)
    {
        ArgumentNullException.ThrowIfNull(method);

        var attr = method.GetCustomAttribute<NativeTypeAttribute>();
        if (attr == null)
            throw new ArgumentException($"Method {method.Name} is not a valid method, should mark it with {typeof(NativeTypeAttribute).Name}.");
        var name = attr.Name ?? method.Name;
        return GetType(name);
    }

    public IEnumerable<NativeType> Primitives => Declarations.Select(x => (NativeType)x.Type).Where(x => x.IsPrimitive);

    public ReferenceType Primitive(string name)
    {
        if (Declarations.TryGetNode(name, out var node))
        {
            if (node.Type is NativeType native && native.IsPrimitive)
                return native.Reference();
        }

        throw new ArgumentException($"Primitive type {name} not found.");
    }
    

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType Record(TypeBase keys, TypeBase type)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(keys, type);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType Partial(TypeBase type)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(type);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType Required(TypeBase type)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(type);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType Readonly(TypeBase type)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(type);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType Pick(TypeBase type, TypeBase keys)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(type, keys);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType Omit(TypeBase type, TypeBase keys)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(type, keys);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType Exclude(UnionType unionType, TypeBase excludedMembers)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(unionType, excludedMembers);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType Extract(TypeBase type, UnionType union)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(type, union);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType NonNullable(TypeBase type)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(type);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType Parameters(TypeBase type)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(type);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType ConstructorParameters(TypeBase type)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(type);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType ReturnType(TypeBase type)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(type);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType InstanceType(TypeBase type)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(type);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType ThisParameterType(TypeBase type)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(type);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType OmitThisParameter(TypeBase type)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(type);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType ThisType(TypeBase type)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(type);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType Uppercase(TypeBase stringType)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(stringType);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType Lowercase(TypeBase stringType)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(stringType);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType Capitalize(TypeBase stringType)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(stringType);
    }

    [NativeType(Category = NativeTypeCategory.TypeScriptUtility)]
    public ReferenceType Uncapitalize(TypeBase stringType)
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference(stringType);
    }

    [NativeType("string", Category = NativeTypeCategory.TypeScriptPrimitive)]
    public ReferenceType String()
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference();
    }

    [NativeType("number", Category = NativeTypeCategory.TypeScriptPrimitive)]
    public ReferenceType Number()
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference();
    }

    [NativeType("boolean", Category = NativeTypeCategory.TypeScriptPrimitive)]
    public ReferenceType Boolean()
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference();
    }

    [NativeType("unknown", Category = NativeTypeCategory.TypeScriptPrimitive)]
    public ReferenceType Unknown()
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference();
    }

    [NativeType("any", Category = NativeTypeCategory.TypeScriptPrimitive)]
    public ReferenceType Any()
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference();
    }

    [NativeType("undefined", Category = NativeTypeCategory.TypeScriptPrimitive)]
    public ReferenceType Undefined()
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference();
    }

    [NativeType("never", Category = NativeTypeCategory.TypeScriptPrimitive)]
    public ReferenceType Never()
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference();
    }

    [NativeType("null", Category = NativeTypeCategory.TypeScriptPrimitive)]
    public ReferenceType Null()
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference();
    }

    [NativeType("symbol", Category = NativeTypeCategory.TypeScriptPrimitive)]
    public ReferenceType Symbol()
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference();
    }

    [NativeType(Category = NativeTypeCategory.JavaScriptNative)]
    public ReferenceType Date()
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference();
    }

    [NativeType(Category = NativeTypeCategory.JavaScriptNative)]
    public ReferenceType Regex()
    {
        return GetType(MethodBase.GetCurrentMethod()).Reference();
    }


}
