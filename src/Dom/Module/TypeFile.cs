using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nabla.TypeScript;

public class TypeFile : NamedDomNode
{
    public const string Extension = ".ts";

    private bool _forceModule;

    public TypeFile(string name)
        : base(name)
    {

        Imports = new(this);
        Declarations = new(this);
        Namespaces = new(this);
        References = new(this);
    }

    public TypeFile(string name, IEnumerable<TypeDeclaration> declarations)
        : this(name)
    {
        Declarations.AddRange(declarations);
    }

    public string? Path { get; set; }

    public string FileName => Name + Extension;

    public override DomNodeKind Kind => DomNodeKind.TypeFile;

    public virtual ModuleReferenceMode ReferenceMode { get; } = ModuleReferenceMode.Explicit;

    public DomNodeCollection<TypeImport> Imports { get; }

    public DomNodeCollection<FileReference> References { get; }

    public NamedDomNodeCollection<TypeDeclaration> Declarations { get; }

    public IEnumerable<TypeDeclaration> AllDeclarations => Declarations.Concat(Namespaces.SelectMany(x => x.Declarations));

    public NamedDomNodeCollection<TypeNamespace> Namespaces { get; }

    public override TypeFile? DeclaringFile => this;

    private IEnumerable<IExportable> Exportables => Declarations.Cast<IExportable>().Concat(Namespaces);

    public bool IsModule => _forceModule || Imports.Any() || Exportables.Where(x => !x.IsLocal).Any();

    public override void Write(TypeWriter writer)
    {
        if (References.Any())
        {
            writer.WriteList(References, WriteListOptions.Statements);
            writer.InsertBlankLine();
        }

        if (Imports.Any())
        {
            writer.WriteList(Imports, WriteListOptions.Statements);
            writer.InsertBlankLine();
        }

        writer.WriteList(Declarations, WriteListOptions.BlockBetween);

        writer.WriteList(Namespaces, WriteListOptions.BlockBetween);
    }

    public void LiftToModule()
    {
        if (_forceModule)
            return;
        
        _forceModule = true;

        foreach (var ns in Namespaces)
        {
            ns.IsLocal = false;
        }

        foreach (var fileRef in References)
        {
            var target = fileRef.Target;

            if (!target.IsModule && target != this)
                target.LiftToModule();

            Imports.Add(TypeImport.ImportAll(target, target.Name.Replace('.', '_')));
        }

        References.Clear();

    }

    protected override void OnAttach(DomNode child)
    {
        bool mayLift = child is TypeImport || child is IExportable exportable && !exportable.IsLocal;

        if (mayLift && !IsModule && Namespaces.Any())
        {
            throw new CodeException($"Cannot attach {child} to non-module file {Name}.");
        }

        if (child is TypeNamespace ns && IsModule)
        {
            ns.IsLocal = false;
        }

        base.OnAttach(child);

       
    }


    public bool IsDefined(string typeName, string? namespc)
    {
        if (namespc != null)
        {
            if (Namespaces.TryGetNode(namespc, out var ns))
                return ns.Declarations.ContainsName(typeName);
        }

        return Declarations.ContainsName(typeName);
    }

    public TypeBase GetExportedType(string name)
    {
        if (TryGetExportedType(name, out var type))
            return type;

        throw new ArgumentException($"Type {name} is not defined in {Name} or not exported.");
    }

    public bool TryGetExportedType(string name, [NotNullWhen(true)]out TypeBase? type)
    {
        if (Declarations.TryGetNode(name, out var declare) && !declare.IsLocal)
        {
            type = declare.Type;
            return true;
        }

        if (TS.TryParseFullName(name, out name, out var namespc))
        {
            if (Namespaces.TryGetNode(namespc, out var ns) && ns.Declarations.TryGetNode(name, out declare))
            {
                type = declare.Type;
                return true;
            }
        }

        type = null;
        return false;
    }

    public TypeBase Import(string typeName, TypeFile module, string? alias = null)
    {
        var type = module.GetExportedType(typeName);

        Import(type, alias);

        return type;
    }

    public Reference Import(TypeBase type, string? alias = null)
    {
        var source = type.DeclaringFile;

        if (source == null)
            throw new InvalidOperationException($"Type {type} has not been declared.");

        if (source == this)
            throw new InvalidOperationException("Cannot import type from self.");

        if (source.ReferenceMode == ModuleReferenceMode.Implicit)
            throw new InvalidOperationException("Importing type form an implicitly referenced module is not necessary.");

        if (source.IsModule)
        {
            if (!IsModule)
                throw new CodeException($"Cannot import type {type.GetName()} from module {source.Name} to non-module {Name}.");

            var import = Imports.FirstOrDefault(x => x.Target == source);
            TypeImportItem? item;

            if (import == null)
            {
                import = new(source);
                Imports.Add(import);
                item = null;
            }
            else
            {
                if (import.All == null)
                {
                    item = import.Members.FirstOrDefault(x => x.Alias == alias && x.IsMatch(type));
                }
                else
                {
                    item = import.All;
                }
            }

            if (item == null)
            {
                var ns = type.GetNamespace();

                if (ns == null)
                {
                    item = new TypeImportType(type, alias);
                }
                else
                {
                    item = new TypeImportNamespace(ns, alias);
                }

                import.Members.Add(item);
            }

            return item;
        }
        else
        {
            if (IsModule)
                throw new CodeException($"Cannot add namespace reference from {source.Name} to module {Name}.");

            var fileRef = References.FirstOrDefault(x => x.Target == source);

            if (fileRef == null)
            {
                fileRef = new(source);
                References.Add(fileRef); 
            }

            return fileRef;
        }
    }

    public TypeDeclaration Add(TypeBase type, bool export = true, bool declare = false, bool resolveImports = true, string? namespc = null, Func<TypeBase, string?>? aliasFactory = null)
    {
        if (type is ReferenceType)
            throw new NotSupportedException("Export type from imported member is not supported yet.");

        if (type is not INamedType namedType)
            throw new InvalidOperationException("Type must have a name.");

        if (type.DeclaringFile != null)
            throw new InvalidOperationException($"Type {type.GetName()} is already declared.");

        if (Declarations.ContainsName(namedType.Name))
            throw new InvalidOperationException($"Type {namedType} has already been declared in module {Name}.");

        TypeDeclaration declaration = new(namedType.Name, type)
        {
            IsLocal = !export,
            IsDeclare = declare,
        };

        ICollection<TypeDeclaration> declarations = Declarations;

        if (namespc != null)
        {
            if (!Namespaces.TryGetNode(namespc, out var ns))
            {
                ns = new(namespc);
                Namespaces.Add(ns);
            }

            declarations = ns.Declarations;
        }

        declarations.Add(declaration);

        if (resolveImports)
            ResolveImports(type, aliasFactory ?? DefaultAliasFactory);

        return declaration;
    }

    private string? DefaultAliasFactory(TypeBase targetType)
    {
        Debug.Assert(targetType.DeclaringFile != null);

        string name = targetType.GetName(), alias = name;
        int seed = 0;

        while (Declarations.ContainsName(alias))
        {
            alias = $"{targetType.DeclaringFile.Name}{name}{seed++:#}".ToTypeName();
        }

        if (alias != null && Declarations.ContainsName(name))
        {
            var import = Imports.Where(x => x.Target == targetType.DeclaringFile).FirstOrDefault();

            if (import != null)
            {
                var item = import.Members.Where(x => x.TargetName == name && x.Alias == null).FirstOrDefault();
                if (item != null)
                    item.Alias = alias;
            }
        }

        if (name == alias)
            return null;
        else
            return alias;

    }

    public void ResolveImports(Func<TypeBase, string?>? aliasFactory = null)
    {
        foreach (var declare in Declarations.Concat(Namespaces.SelectMany(x => x.Declarations)).ToArray())
        {
            ResolveImports(declare.Type, aliasFactory ?? DefaultAliasFactory);
        }
    }

    private void ResolveImports(TypeBase type, Func<TypeBase, string?> aliasFactory)
    {
        List<(ReferenceType, TypeImportItem)> list = new();

        type.WalkDescendents(node =>
        {
            if (node is ReferenceType refType && refType.Reference is not TypeImportItem)
            {
                var refModule = refType.Reference.Target.DeclaringFile;

                Debug.Assert(refModule != null);

                if (refModule != this && refModule.ReferenceMode == ModuleReferenceMode.Explicit)
                {
                    if (refType.Reference.Target is TypeBase targetType)
                    {
                        var importRef = Import(targetType, aliasFactory(targetType));

                        if (importRef is TypeImportItem item)
                            list.Add((refType, item));
                    }
                }
            }
        });

        foreach (var (old, import) in list)
        {
            old.Inject(@ref => new ImportProxy(import, (TypeReference)@ref));
        }

    }

    public string GetPath(bool withExtension)
    {
        var path = Path;

        if (path == null)
            path = $"./{(withExtension ? FileName : Name)}";
        else if (!withExtension && path.EndsWith(Extension))
            path = path[..^3];

        return path;
    }

    private static readonly char[] _directorySeparators = new[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar };

    public string GetPath(TypeFile relativeTo, bool withExtension)
    {
        var toPath = new Uri(System.IO.Path.GetFullPath(relativeTo.GetPath(withExtension)));
        var mePath = new Uri(System.IO.Path.GetFullPath(GetPath(withExtension)));

        var path = toPath.MakeRelativeUri(mePath).ToString();

        if (path.IndexOfAny(_directorySeparators) < 0)
            path = "./" + path;

        return path;
    }

    public override string ToString()
    {
        return $"{GetType().Name}: {Name}";
    }
}
