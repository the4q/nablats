using System.Reflection;

namespace Nabla.TypeScript.Tool.Reflection;

public class ReflectionFileFactory : FileFactory
{
    private readonly Assembly _assembly;

    public ReflectionFileFactory(Assembly assembly, ISerializationInfo serializationInfo, FileOrganizer organizer, CodeOptions options, TypeDiscoveryStrategy strategy)
        : base(serializationInfo, organizer, options)
    {
        _assembly = assembly;
        Strategy = strategy;
    }

    public TypeDiscoveryStrategy Strategy { get; }

    public string? DiscovererTypeName { get; init; }

    protected override ITypeSourceDiscoverer CreateDiscoverer()
    {
        return ReflectionTypeDiscoverer.Create(_assembly, Strategy, DiscovererTypeName);
    }

    protected override ITypeFactory CreateTypeFactory()
    {
        return new TypeFactory<Type>(this, new ReflectionSourceDescriptor(SerializationInfo, Options));
    }

    protected override string? ResolveDesiredModuleName(object source)
    {
        if (source is Type clrType)
        {
            return clrType.CascadeGetCustomAttribute<TsFileNameAttribute>()?.Name;
        }

        throw new ArgumentException("Invalid source type, requires System.Type.");
    }
}
