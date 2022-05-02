using System.Reflection;

namespace Nabla.TypeScript.Tool.Reflection;

public abstract class ReflectionTypeDiscoverer : ITypeSourceDiscoverer
{
    private readonly Assembly _assembly;

    public ReflectionTypeDiscoverer(Assembly assembly)
    {
        _assembly = assembly;
    }

    protected abstract IEnumerable<Type> Discover(Assembly assembly);

    public IEnumerable<Type> Discover()
    {
        OnStart();
        return Discover(_assembly);
    }

    protected virtual void OnStart()
    {

    }

    protected static bool ShouldAvoid(Type type)
    {
        if (type == typeof(void) || type == typeof(object))
            return true;

        var primitive = TypeUtils.GetPrimitiveType(type);

        if (primitive != null)
            return true;

        if (type.IsDefined(typeof(TsIgnoreAttribute)))
            return true;

        return false;
    }

    public static ITypeSourceDiscoverer Create(Assembly assembly, TypeDiscoveryStrategy strategy, string? discovererTypeName)
    {
        Type? discovererType;

        if (discovererTypeName != null)
        {
            discovererType = assembly.GetType(discovererTypeName, true);
        }
        else
        {
            discovererType = assembly.GetCustomAttribute<TsTypeDiscovererAttribute>()?.DiscovererType;
        }

        if (discovererType != null)
        {
            if (discovererType.GetInterface(typeof(ITypeSourceDiscoverer).Name) != typeof(ITypeSourceDiscoverer))
            {
                throw new CodeException($"{discovererType} is not a type source discoverer.");
            }

            try
            {
                return (ITypeSourceDiscoverer)TypeUtils.CreateInstance(discovererType, assembly)!;
            }
            catch (Exception ex)
            {
                throw new CodeException($"Failed to create type source discoverer {discovererType}: {ex.Message}", ex);
            }
        }

        if (strategy == TypeDiscoveryStrategy.Auto && WebApiTypeDiscoverer.IsAspNetCoreAssembly(assembly))
        {
            strategy = TypeDiscoveryStrategy.WebApi;
        }

        if (strategy == TypeDiscoveryStrategy.WebApi)
            return new WebApiTypeDiscoverer(assembly);

        return new DataAnnotationTypeDiscoverer(assembly);
    }

    IEnumerable<object> ITypeSourceDiscoverer.Discover()
    {
        return Discover();
    }
}

