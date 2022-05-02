using System.Reflection;

namespace Nabla.TypeScript.Tool;

public interface ISerializationInfo
{
    string? GetDedicatedFieldName(MemberInfo member);

    string ExecutePolicy(string name, PropertyNamingPolicy namingPolicy);

    bool IsVariantType(Type type);

    bool IsIgnored(MemberInfo member);
}

