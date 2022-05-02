using System.Diagnostics.CodeAnalysis;

namespace Nabla.TypeScript.Tool;

public interface ISourceDescriptor<TSource>
    where TSource : notnull
{
    bool IsTypeDefinition(TSource source);

    TSource GetTypeDefinition(TSource source);

    bool IsGenericType(TSource source);

    ICollection<TSource> GetGenericArguments(TSource source);

    EnumTypeInfo? GetEnumInfo(TSource source);

    string GetName(TSource source);

    bool IsTypeParameter(TSource source);

    bool IsClrTuple(TSource source, [NotNullWhen(true)] out TSource? tupleType, [NotNullWhen(true)]out TSource[]? tupleArgs);

    bool IsTypeScriptTuple(TSource source);

    string Describe(TSource source);

    string ResolveTypeName(TSource source);

    string ResolvePropertyName(IPropertyMetaProvider<TSource> meta);

    TSource? GetBaseType(TSource source);

    IEnumerable<IPropertyMetaProvider<TSource>> GetProperties(TSource source, bool includeBaseTypes);

    IPropertyRelatedMetaProvider<TSource> CreateGenericArgumentMetaProvider(IPropertyMetaProvider<TSource> property, int argumentIndex);

    IPropertyRelatedMetaProvider<TSource> CreateArrayElementMetaProvider(IPropertyMetaProvider<TSource> property);

    TSource GetUnderlyingSource(TSource source);

    bool IsVariant(TSource source);

    VariantTypeInfo GetVariantInfo(TSource source, IMetaProvider<TSource> meta);

    RecordTypeInfo<TSource>? GetDictionaryInfo(TSource source);

    TypeScriptPrimitive? GetPrimitive(TSource source, IMetaProvider<TSource>? meta);

    bool IsPrimitive(TSource source);

    CollectionInfo<TSource>? GetCollectionInfo(TSource source);

    bool IsRequired(TSource source);
}

public class CollectionInfo<TSource>
    where TSource: notnull
{
    public CollectionInfo(bool isArray, TSource elementType)
    {
        IsArray = isArray;
        ElementType = elementType;
    }

    public bool IsArray { get; }

    public TSource ElementType { get; }
}