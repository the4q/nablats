using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nabla.TypeScript.Tool.Reflection
{
    internal class ReflectionSourceDescriptor : ISourceDescriptor<Type>
    {
        private readonly ISerializationInfo _serialization;

        public ReflectionSourceDescriptor(ISerializationInfo serialization)
        {
            _serialization = serialization;
        }

        public IPropertyRelatedMetaProvider<Type> CreateArrayElementMetaProvider(IPropertyMetaProvider<Type> property)
        {
            var site = (MemberSite)property;
            return site.ArrayElement();
        }

        public IPropertyRelatedMetaProvider<Type> CreateGenericArgumentMetaProvider(IPropertyMetaProvider<Type> property, int argumentIndex)
        {
            return ((MemberSite)property).GenericArgument(argumentIndex);
        }

        public string GetFullName(Type source)
        {
            return source.FullName!;
        }

        public Type? GetBaseType(Type source)
        {
            return TypeUtils.GetBaseType(source);
        }

        public CollectionInfo<Type>? GetCollectionInfo(Type source)
        {
            if (TypeUtils.IsGenericDictionary(source, out _, out _))
                return null;

            if (TypeUtils.IsCollection(source, out var collInfo))
                return collInfo;

            return null;
        }

        public RecordTypeInfo<Type>? GetDictionaryInfo(Type source)
        {
            if (TypeUtils.IsGenericDictionary(source, out var key, out var value))
            {
                return new(key, value);
            }

            return null;
        }

        public EnumTypeInfo? GetEnumInfo(Type source)
        {
            if (source.IsEnum)
            {
                var names = Enum.GetNames(source);
                var values = Enum.GetValues(source);

                var attr = source.CascadeGetCustomAttribute<TsEnumHandlingAttribute>();
                Dictionary<string, long> members = new(names.Select((x, i) =>
                    new KeyValuePair<string, long>(x, Convert.ToInt64(values.GetValue(i)))));

                return new(members, attr?.Handling, attr?.GetNamingPolicy());
            }

            return null;
        }

        public ICollection<Type> GetGenericArguments(Type source)
        {
            return source.GetGenericArguments();
        }

        public string GetName(Type source)
        {
            return source.Name;
        }

        public bool IsPrimitive(Type source)
        {
            return TypeUtils.GetPrimitiveType(source).HasValue;
        }

        public TypeScriptPrimitive? GetPrimitive(Type source, IMetaProvider<Type>? meta)
        {
            return TypeUtils.GetPrimitiveType(source);
        }

        public DateHandling? GetDateHandling(IMetaProvider<Type> meta)
        {
            return (meta as MemberSite)?.Member
                        .CascadeGetCustomAttribute<TsDateHandlingAttribute>()?
                        .Handling;
        }

        public IEnumerable<IPropertyMetaProvider<Type>> GetProperties(Type source, bool includeBaseTypes)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            if (!includeBaseTypes)
                bindingFlags |= BindingFlags.DeclaredOnly;

            foreach (var member in source.GetMembers(bindingFlags))
            {
                if (member is not PropertyInfo or FieldInfo)
                    continue;

                yield return MemberSite.Create(member);
            }

        }

        public bool IsPropertyIgnored(IPropertyMetaProvider<Type> meta)
        {
            if (meta is MemberSite member)
            {
                return _serialization.IsIgnored(member.Member) || member.Member.IsDefined(typeof(TsIgnoreAttribute));
            }

            return false;
        }

        public Type GetTypeDefinition(Type source)
        {
            if (source.IsTypeDefinition)
                return source;

            if (source.IsGenericType)
            {
                if (!source.IsGenericTypeDefinition)
                    return source.GetGenericTypeDefinition();

                return source;
            }

            throw new ArgumentException($"{source} is not a type definition.");
        }

        public Type GetUnderlyingSource(Type source)
        {
            return Nullable.GetUnderlyingType(source) ?? source;
        }

        public bool IsVariant(Type clrType)
        {
            return clrType == typeof(object) || _serialization.IsVariantType(clrType);
        }

        public VariantTypeInfo GetVariantInfo(Type clrType, IMetaProvider<Type> meta)
        {
            var attr = (meta as MemberSite)?.Member.GetCustomAttribute<TsVariantTypeHandlingAttribute>();
            return new(attr?.Handling ?? VariantTypeHandling.AsUnknown, attr?.GenericParameterName);
        }

        public bool IsGenericType(Type source)
        {
            return source.IsGenericType;
        }

        public bool IsRequired(Type source)
        {
            return source.IsDefined(typeof(TsExportAttribute));
        }

        public bool IsClrTuple(Type source, [NotNullWhen(true)] out Type? tupleType, [NotNullWhen(true)] out Type[]? tupleArgs)
        {
            if (TypeUtils.IsTuple(source, out tupleArgs))
            {
                tupleType = source.IsValueType ? source.Assembly.GetType("System.Tuple`" + tupleArgs.Length)! : source;
                return true;
            }

            tupleType = null;
            return false;
        }

        public bool IsTypeScriptTuple(Type source)
        {
            return source.IsDefined(typeof(TsTupleAttribute));
        }

        public int? GetTupleOrder(IPropertyMetaProvider<Type> property)
        {
            var member = property as MemberSite;

            return member?.Member.GetCustomAttribute<TsTupleOrderAttribute>()?.Order;

        }

        public bool IsTypeDefinition(Type source)
        {
            return source.IsTypeDefinition;
        }

        public bool IsTypeParameter(Type source)
        {
            return source.IsGenericParameter;
        }


        public string? GetDedicatedPropertyName(IPropertyMetaProvider<Type> meta)
        {
            var property = (MemberSite)meta;
            return _serialization.GetDedicatedFieldName(property.Member);

        }

        private static string ResolveTypeName(Type clrType, TsTypeNameAttribute? attr)
        {
            string? name = attr?.Name;

            if (string.IsNullOrEmpty(name))
            {
                int p;

                while (true)
                {
                    string n = clrType.Name;
                    p = n.IndexOf('`');

                    if (p > 0)
                        n = n[..p];

                    name = n + name;

                    if (!clrType.IsNested)
                        break;

                    clrType = clrType.DeclaringType!;
                }
            }

            return name;
        }

        public string ResolveTypeName(Type source)
        {
            return ResolveTypeName(source, source.GetCustomAttribute<TsTypeNameAttribute>(false));
        }

        public ITypeOverrideInfo? GetOverridingInfo(IPropertyMetaProvider<Type> meta)
        {
            return (meta as MemberSite)?.Member.GetCustomAttribute<TsTypeOverrideAttribute>();
        }

        public bool IsTypeIgnored(Type source)
        {
            return source.IsDefined(typeof(TsIgnoreAttribute));
        }

    }
}
