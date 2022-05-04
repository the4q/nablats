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

    int? GetTupleOrder(IPropertyMetaProvider<TSource> property);

    string GetFullName(TSource source);

    string ResolveTypeName(TSource source);

    string? GetDedicatedPropertyName(IPropertyMetaProvider<TSource> meta);

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

    ITypeOverrideInfo? GetOverridingInfo(IPropertyMetaProvider<TSource> meta);

    bool IsTypeIgnored(TSource source);

    bool IsPropertyIgnored(IPropertyMetaProvider<TSource> meta);

    DateHandling? GetDateHandling(IMetaProvider<TSource> meta);

}
