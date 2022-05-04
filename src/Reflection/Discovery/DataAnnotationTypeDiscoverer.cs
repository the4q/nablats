using System.Reflection;

namespace Nabla.TypeScript.Tool.Reflection;

internal sealed class DataAnnotationTypeDiscoverer : ReflectionTypeDiscoverer
{
    public DataAnnotationTypeDiscoverer(Assembly entrance)
        : base(entrance)
    {
    }

    protected override void OnStart()
    {
        UI.Say("Discover POCO types by data annotations.");
        base.OnStart();
    }

    protected override IEnumerable<Type> Discover(Assembly assembly)
    {
        return assembly.GetExportedTypes()
            .Where(IsHit);
    }

    private static bool IsHit(Type type)
    {
        return !ShouldAvoid(type)
            && type.GetCustomAttribute<TsExportAttribute>() != null
            && type.GetCustomAttribute<TsIgnoreAttribute>() == null;
    }
}

