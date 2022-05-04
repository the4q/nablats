using System.Reflection;

namespace Nabla.TypeScript.Tool.Reflection;

public interface ISerializationInfo : IPropertyNameResolver
{
    string? GetDedicatedFieldName(MemberInfo member);

    bool IsVariantType(Type type);

    bool IsIgnored(MemberInfo member);
}

