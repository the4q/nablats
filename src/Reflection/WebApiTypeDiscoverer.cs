using Nabla.TypeScript.Tool.Reflection;
using System.Reflection;

namespace Nabla.TypeScript.Tool.Reflection;

internal class WebApiTypeDiscoverer : ReflectionTypeDiscoverer
{
    public WebApiTypeDiscoverer(Assembly assembly)
        : base(assembly)
    {
    }

    protected override void OnStart()
    {
        UI.Say("Discover POCO types by AspNetCore Web API pattern.");
        base.OnStart();
    }

    protected override IEnumerable<Type> Discover(Assembly assembly)
    {

#if DEBUG
        if (UI.DebugMode)
            UI.Pause();
#endif
        HashSet<Type> types = new();
        
        foreach (var type in assembly.GetExportedTypes())
        {
            if (IsApiController(type))
            {
                UI.Comment($"Found controller {type.FullName}");

                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (IsAction(method))
                    {
                        foreach (var param in method.GetParameters().Select(x => x.ParameterType).Append(method.ReturnType))
                        {
                            var poco = GetPocoType(param);
                            if (poco != null && !types.Contains(poco))
                            {
                                types.Add(poco);
                                yield return poco;
                            }
                        }
                    }
#if DEBUG
                    else
                    {
                        UI.Comment($"{method} is not an action.");
                    }
#endif
                }
            }
        }
    }

    private static Type? GetPocoType(Type type)
    {
        if (ShouldAvoid(type))
            return null;

        type = StripWrappers(type);

        if (TypeUtils.IsGenericDictionary(type, out _, out var valueType))
        {
            return GetPocoType(valueType);
        }

        if (TypeUtils.IsCollection(type, out var collInfo))
        {
            return GetPocoType(collInfo.ElementType);
        }

        if (!ShouldAvoid(type) && (TypeUtils.HasParameterlessConstructor(type) || TypeUtils.IsRecordType(type)))
        {
            UI.Comment($"Found POCO type {type.FullName}");
            return TypeUtils.GetTypeDefinition(type);
        }
#if DEBUG
        else
        {
            UI.Comment($"{type.FullName} is not an POCO type.");
        }
#endif

        return null;
    }

    private static readonly Type[] _genericWrapperType = new[]
    {
        typeof(Task<>),
        typeof(ValueTask<>),
    };

    private const string ControllerBase = "Microsoft.AspNetCore.Mvc.ControllerBase";
    private const string ApiControllerAttribute = "Microsoft.AspNetCore.Mvc.ApiControllerAttribute";
    private const string RouteAttribute = "Microsoft.AspNetCore.Mvc.RouteAttribute";
    private const string HttpMethodAttribute = "Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute";
    private const string MvcAssemblyName = "Microsoft.AspNetCore.Mvc";

    private static readonly string[] _genericWrapperTypeNames = new[]
    {
        "Microsoft.AspNetCore.Mvc.ActionResult`1"
    };

    

    private static Type StripWrappers(Type type)
    {
        if (type.IsGenericType)
        {
            // strip wrapper types

            var def = type.GetGenericTypeDefinition();

            foreach (var wrapper in _genericWrapperType)
            {
                if (def == wrapper || def.IsSubclassOf(wrapper) || wrapper.IsInterface && def.GetInterfaces().Contains(def))
                {
                    return StripWrappers(type.GetGenericArguments()[0]);
                }
            }

            foreach (var wrapper in _genericWrapperTypeNames)
            {
                if (def.FullName == wrapper)
                    return StripWrappers(type.GetGenericArguments()[0]);
            }
        }

        return type;
    }

    private static bool IsApiController(Type type)
    {
        //if (type.IsSubclassOf(typeof(ControllerBase)))
        //{
        //    return type.IsDefined(typeof(ApiControllerAttribute));
        //}

        if (type.IsValueType || type.IsAbstract || type.IsArray || type.IsInterface)
            return false;
        
        if (TypeUtils.IsTypeOrSubclass(type, ControllerBase))
        {
            return TypeUtils.IsCustomAttributeDefined(type, ApiControllerAttribute);
        }


        return false;
    }


    private static bool IsAction(MethodInfo method)
    {
        //return method.IsDefined(typeof(RouteAttribute));

        return TypeUtils.IsCustomAttributeDefined(method, RouteAttribute) || TypeUtils.IsCustomAttributeDefined(method, HttpMethodAttribute);
    }

    internal static bool IsAspNetCoreAssembly(Assembly assembly)
    {
        var names = assembly.GetReferencedAssemblies();

        return names.Any(name => name.Name == MvcAssemblyName);
    }
}
