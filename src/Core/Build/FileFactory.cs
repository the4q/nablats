using System.Diagnostics;

namespace Nabla.TypeScript.Tool;

public abstract class FileFactory : IFileFactory
{

    private readonly Dictionary<string, TypeFile> _files;
    private readonly FileOrganizer _organizer;
    private readonly Dictionary<string, TypeBase> _utilities;
    private Dictionary<string, TypeFile>? _shadows;

    public FileFactory(FileOrganizer organizer, CodeOptions options)
    {
        _organizer = organizer;
        Options = options;
        _files = new();
        _utilities = new();

    }

    public ICollection<TypeFile> Files => _files.Values;

    public CodeOptions Options { get; }

    public Mapping.MappingSchema? Mapping { get; init; }

    public ICollection<TypeFile> CreateFiles()
    {
        _files.Clear();

        CreateTypes();

        ResolveUtilityModules();

        foreach (var module in _files.Values)
        {
            module.ResolveImports();
        }

        return Files;
    }

    public void AddType(TypeBase type, object source)
    {
        var name = _organizer.ResolveFileName(source, GetDesiredModuleName(source));
        var module = EnsureModule(name);
        string? namespc;

        if (Options.UseNamespaces)
            namespc = GetNamespace(source);
        else
            namespc = null;

        if (module.IsDefined(type.GetName(), namespc))
            throw new CodeException($"Duplicated type name found {TS.PopulateFullName(type.GetName(), namespc)} in module {name}.");

        module.Add(type, true, resolveImports: false, namespc: namespc);

    }

    public ReferenceType UseUtility(Func<TypeBase> factory, params TypeBase[] arguments)
    {
        string typeName = factory.Method.Name;

        if (!_utilities.TryGetValue(typeName, out var type))
        {
            type = factory();
            _utilities[typeName] = type;
        }

        return TS.CreateReference(type, arguments);
    }

    private void ResolveUtilityModules()
    {
        if (_utilities.Count > 0)
        {
            if (!Options.UseNamespaces || _organizer.ProduceSingleFile)
            {
                foreach (var type in _utilities.Values)
                {
                    AddType(type, UtilityFactory.Token);
                }
            }
            else
            {
                var namespc = typeof(UtilityFactory).Namespace!;
                var file = new TypeFile(UtilityFactory.ModuleName);

                foreach (var type in _utilities.Values)
                {
                    file.Add(type, namespc: namespc);
                }

                _files.Add(file.Name, file);
            }
        }
    }

    private string? GetDesiredModuleName(object source)
    {
        if (source == UtilityFactory.Token)
            return UtilityFactory.ModuleName;

        if (Mapping?.Files != null)
        {
            var name0 = _organizer.NameResolver.ResolveFileName(source);
            var mapping = Mapping.GetFileMapping(name0);

            if (mapping?.Target != null)
                return mapping.Target;
        }

        return ResolveDesiredModuleName(source);
    }

    private string? GetNamespace(object source)
    {
        if (source == UtilityFactory.Token)
            return typeof(UtilityFactory).Namespace;

        return _organizer.NameResolver.ResolveNamespace(source);
    }

    protected abstract string? ResolveDesiredModuleName(object source);

    protected abstract ITypeSourceDiscoverer CreateDiscoverer();

    protected abstract ITypeFactory CreateTypeFactory();

    private void CreateTypes()
    {
        var discoverer = CreateDiscoverer();
        var factory = CreateTypeFactory();

        foreach (var source in discoverer.Discover())
        {
            if (_organizer.DefaultFileName == null)
                _organizer.DefaultFileName = _organizer.NameResolver.ResolveFileName(source);

            factory.CreateType(source);
        }
        
    }

    protected TypeFile EnsureModule(string name)
    {
        if (!_files.TryGetValue(name, out var module))
        {
            module = new(name);
            _files.Add(name, module);
        }

        return module;
    }

    internal ShadowType UseShadowType(string moduleName, string typeName, Func<string, ShadowType> factory)
    {
        var module = EnsureShadowModule(moduleName);

        if (!module.TryGetExportedType(typeName, out var type))
        {
            string? namespc, name;


            if (Options.UseNamespaces)
            {
                if (!TS.TryParseFullName(typeName, out name, out namespc))
                    throw new CodeException("When generating code in namespace mode, all third-party types should be assumed as namespaced type. Thus type name passed in must be in full name style, like 'some.namespace.type'.");

            }
            else
            {
                name = typeName;
                namespc = null;
            }

            type = factory(name);
            module.Add(type, namespc: namespc);
        }

        return (ShadowType)type;
    }

    public TypeFile EnsureShadowModule(string path)
    {
        if (_shadows == null)
            _shadows = new();

        ParseModuleName(path, out var name, out path, out var exists);

        if (!_shadows.TryGetValue(path, out var module))
        {
            module = new(name) { Path = path };
            _shadows.Add(path, module);
        }

        if (!exists)
            UI.Warning($"Third-party module file {Path.GetFullPath(path)} does not exists.");

        return module;
    }

    private static void ParseModuleName(string value, out string name, out string path, out bool exists)
    {
        if (!value.ToLower().EndsWith(TypeFile.Extension))
            value += TypeFile.Extension;

        name = Path.GetFileNameWithoutExtension(value);
        path = value;
        exists = File.Exists(path);

    }
}
