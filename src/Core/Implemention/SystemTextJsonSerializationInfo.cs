using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nabla.TypeScript.Tool;

public sealed class SystemTextJsonSerializationInfo : ISerializationInfo
{
    private static readonly Type[] _variantTypes = new Type[]
    {
        typeof(JsonDocument),
        typeof(JsonElement),
        typeof(System.Text.Json.Nodes.JsonNode)
    };

    public string ExecutePolicy(string name, PropertyNamingPolicy policy)
    {
        if (policy == PropertyNamingPolicy.CamelCase)
            return JsonNamingPolicy.CamelCase.ConvertName(name);

        return name;
    }

    public string? GetDedicatedFieldName(MemberInfo member)
    {
        return member.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
    }

    public bool IsIgnored(MemberInfo member)
    {
        return member.IsDefined(typeof(JsonIgnoreAttribute));
    }

    public bool IsVariantType(Type type)
    {
        return _variantTypes.Any(x => type == x || type.IsSubclassOf(x));
    }
}