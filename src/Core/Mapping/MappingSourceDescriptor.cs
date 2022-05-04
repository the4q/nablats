using System.Diagnostics.CodeAnalysis;

namespace Nabla.TypeScript.Tool.Mapping
{
    internal sealed class MappingSourceDescriptor<TSource> : ISourceDescriptor<TSource>
        where TSource : notnull
    {
        private readonly ISourceDescriptor<TSource> _original;
        private readonly MappingSchema _schema;

        public MappingSourceDescriptor(ISourceDescriptor<TSource> original, MappingSchema schema)
        {
            _original = original;
            _schema = schema;
            _schema.Prepare();
        }

        public IPropertyRelatedMetaProvider<TSource> CreateArrayElementMetaProvider(IPropertyMetaProvider<TSource> property)
        {
            return _original.CreateArrayElementMetaProvider(property);
        }

        public IPropertyRelatedMetaProvider<TSource> CreateGenericArgumentMetaProvider(IPropertyMetaProvider<TSource> property, int argumentIndex)
        {
            return _original.CreateGenericArgumentMetaProvider(property, argumentIndex);
        }

        public string GetFullName(TSource source)
        {
            return _original.GetFullName(source);
        }

        public TSource? GetBaseType(TSource source)
        {
            return _original.GetBaseType(source);
        }

        public CollectionInfo<TSource>? GetCollectionInfo(TSource source)
        {
            return _original.GetCollectionInfo(source);
        }

        public RecordTypeInfo<TSource>? GetDictionaryInfo(TSource source)
        {
            return _original.GetDictionaryInfo(source);
        }

        public EnumTypeInfo? GetEnumInfo(TSource source)
        {
            var info = _original.GetEnumInfo(source);

            if (info != null)
            {
                var mapping = GetTypeMapping(source)?.Enum;
                var mapping1 = _schema.Defaults?.Enum;

                if (mapping != null || mapping1 != null)
                {
                    info.NamingPolicy = mapping?.GetNamingPolicy() ?? mapping1?.GetNamingPolicy();
                    info.Handling = mapping?.GetHandling() ?? mapping1?.GetHandling();
                }
            }

            return info;
        }

        public ICollection<TSource> GetGenericArguments(TSource source)
        {
            return _original.GetGenericArguments(source);
        }

        public string GetName(TSource source)
        {
            return _original.GetName(source);
        }

        public TypeScriptPrimitive? GetPrimitive(TSource source, IMetaProvider<TSource>? meta)
        {
            return _original.GetPrimitive(source, meta);
        }

        public IEnumerable<IPropertyMetaProvider<TSource>> GetProperties(TSource source, bool includeBaseTypes)
        {
            return _original.GetProperties(source, includeBaseTypes);
        }

        public TSource GetTypeDefinition(TSource source)
        {
            return _original.GetTypeDefinition(source);
        }

        public TSource GetUnderlyingSource(TSource source)
        {
            return _original.GetUnderlyingSource(source);
        }

        public VariantTypeInfo GetVariantInfo(TSource source, IMetaProvider<TSource> meta)
        {
            var property = meta.GetProperty();

            if (property != null)
            {
                var mapping = GetPropertyMapping(property);

                if (mapping?.Variant != null)
                    return mapping.Variant.CreateVairantInfo();
            }

            return _original.GetVariantInfo(source, meta);
        }

        public bool IsClrTuple(TSource source, [NotNullWhen(true)] out TSource? tupleType, [NotNullWhen(true)] out TSource[]? tupleArgs)
        {
            return _original.IsClrTuple(source, out tupleType, out tupleArgs);
        }

        public bool IsGenericType(TSource source)
        {
            return _original.IsGenericType(source);
        }

        public bool IsPrimitive(TSource source)
        {
            return _original.IsPrimitive(source);
        }

        public bool IsRequired(TSource source)
        {
            return _original.IsRequired(source);
        }

        public bool IsTypeDefinition(TSource source)
        {
            return _original.IsTypeDefinition(source);
        }

        public bool IsTypeParameter(TSource source)
        {
            return _original.IsTypeParameter(source);
        }

        public bool IsTypeScriptTuple(TSource source)
        {
            if (GetTypeMapping(source)?.UseTuple == true)
                return true;

            return _original.IsTypeScriptTuple(source);
        }

        public int? GetTupleOrder(IPropertyMetaProvider<TSource> meta)
        {
            var property = GetPropertyMapping(meta);

            return property?.TupleOrder ?? _original.GetTupleOrder(meta);
        }

        public bool IsVariant(TSource source)
        {
            return _original.IsVariant(source);
        }

        public string? GetDedicatedPropertyName(IPropertyMetaProvider<TSource> meta)
        {
            return GetPropertyMapping(meta)?.Target ?? _original.GetDedicatedPropertyName(meta);
        }

        public string ResolveTypeName(TSource source)
        {
            return (GetTypeMapping(source)?.Target) ?? _original.ResolveTypeName(source);
        }

        private TypeMapping? GetTypeMapping(TSource source)
        {
            string key = GetFullName(source);
            return _schema.GetTypeMapping(key);
        }

        private PropertyMapping? GetPropertyMapping(IPropertyMetaProvider<TSource> meta)
        {
            return GetTypeMapping(meta.DeclaringSource)?.GetProperty(meta.Name);
        }

        public ITypeOverrideInfo? GetOverridingInfo(IPropertyMetaProvider<TSource> meta)
        {
            return GetPropertyMapping(meta)?.Overriding ?? _original.GetOverridingInfo(meta);
        }

        public bool IsTypeIgnored(TSource source)
        {
            var mapping = GetTypeMapping(source);

            if (mapping != null && !mapping.Export)
                return true;

            return _original.IsTypeIgnored(source);
        }

        public bool IsPropertyIgnored(IPropertyMetaProvider<TSource> meta)
        {
            var mapping = GetPropertyMapping(meta);

            if (mapping != null && !mapping.Export)
                return true;

            return _original.IsPropertyIgnored(meta);
        }

        public DateHandling? GetDateHandling(IMetaProvider<TSource> meta)
        {
            IDateMapping? property = null, type = null, defaults;
            var pmeta = meta.GetProperty();

            defaults = _schema.Defaults;

            if (pmeta != null)
            {
                type = GetTypeMapping(pmeta.DeclaringSource);

                if (type != null)
                    property = ((TypeMapping)type).GetProperty(pmeta.Name);
            }

            return property?.GetDateHandling() ??
                type?.GetDateHandling() ??
                defaults?.GetDateHandling() ??
                _original.GetDateHandling(meta);
        }
    }
}
