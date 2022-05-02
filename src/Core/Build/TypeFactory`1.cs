using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabla.TypeScript.Tool;

public class TypeFactory<TSource> : ITypeFactory
    where TSource : notnull
{
    private readonly Dictionary<TSource, TypeBase> _definitions;
    private readonly List<DeferredReference<TSource>> _unsolved;

    private readonly Stack<TSource> _buildingDefinitions;
    private readonly ISourceDescriptor<TSource> _descriptor;
    private readonly TypeCreator<TSource>[] _pipeline;

    public TypeFactory(FileFactory factory, ISourceDescriptor<TSource> descriptor)
    {
        FileFactory = factory;
        _definitions = new();
        _unsolved = new();
        _buildingDefinitions = new();
        _descriptor = descriptor;
        _pipeline = InitializeTypeCreators().ToArray();
    }

    public ISourceDescriptor<TSource> Descriptor => _descriptor;

    public FileFactory FileFactory { get; }

    public ISerializationInfo SerializationInfo => FileFactory.SerializationInfo;

    protected virtual IEnumerable<TypeCreator<TSource>> InitializeTypeCreators()
    {
        yield return new TypeParameterCreator<TSource>(this);
        yield return new EnumCreator<TSource>(this);
        yield return new PrimitiveCreator<TSource>(this);
        yield return new VariantCreator<TSource>(this);
        yield return new RecordCreator<TSource>(this);
        yield return new ArrayCreator<TSource>(this);
        yield return new TupleCreator<TSource>(this);
        yield return new InterfaceCreator<TSource>(this);
    }

    TypeBase ITypeFactory.CreateType(object source)
    {
        if (source is not TSource source1)
            throw new ArgumentException($"Source type {source.GetType()} is not acceptable, requires {typeof(TSource)}.");

        return CreateType(source1);
    }

    public virtual TypeBase CreateType(TSource source)
    {
        return CreateDefinition(source);
    }

    public TypeBase Define(TSource source, Func<TypeBase> factory)
    {
        bool tryFactory(TSource s, [NotNullWhen(true)] out TypeBase? t)
        {
            t = factory();
            return true;
        }

        if (TryDefine(source, tryFactory, out var type))
        {
            return type;

        }

        throw new InvalidOperationException("Unable to create type definition.");
    }

    private bool TryDefine(TSource source, TryCreateTypeDelegate factory, [NotNullWhen(true)] out TypeBase? type)
    {
        if (_definitions.TryGetValue(source, out type))
            return true;

        if (factory(source, out type))
        {
            if (type is ReferenceType)
                throw new InvalidOperationException("Cannot export a reference type");


            _definitions.Add(source, type);

            SolveDefers(source);

            FileFactory.AddType(type, source);

            return true;
        }

        return false;
    }


    private void SolveDefers(TSource filter)
    {
        if (_unsolved.Count > 0)
        {
            var unsolved = _unsolved.ToList();
            _unsolved.Clear();
            foreach (var item in unsolved)
            {
                if (filter == null || item.Source.Equals(filter))
                {
                    SolveDefer(item);
                }
                else
                {
                    _unsolved.Add(item);
                }
            }

        }
    }

    private void SolveDefer(DeferredReference<TSource> defer)
    {
        if (_descriptor.IsTypeDefinition(defer.Source))
        {
            //BuildDefinition(defer.ClrType);
            Debug.Assert(_definitions.ContainsKey(defer.Source));

            var type = CreateReference(defer.Meta.Source, defer.Meta);

            Debug.Assert(type is ReferenceType);

            defer.Solve((ReferenceType)type);
        }
        else
            throw new InvalidOperationException("Unable to solve defered type " + _descriptor.Describe(defer.Source));

    }

    internal protected ReferenceType Defer(TSource source, IMetaProvider<TSource> meta)
    {
        if (!_descriptor.IsTypeDefinition(source))
            throw new InvalidOperationException("Defer requires a type definition type.");

        var type = new DeferredReference<TSource>(source, meta);
        _unsolved.Add(type);
        return type.CreateType();
    }

    protected TypeBase CreateDefinition(TSource source)
    {
        if (!_descriptor.IsTypeDefinition(source))
            throw new ArgumentException($"{_descriptor.Describe(source)} is not a type definition.", nameof(source));

        return Define(source, () =>
        {
            _buildingDefinitions.Push(source);

            TypeBase? type = null;

            foreach (var creator in _pipeline.OfType<TypeDefinitionCreator<TSource>>())
            {
                var hit = creator.HitTest(source);

                if (hit)
                {
                    type = creator.CreateDefinition(source, hit.State);
                    break;
                }
            }

            if (type == null)
                throw new InvalidOperationException($"Unable to create type definition for {Descriptor.Describe(source)}");

            var lastest = _buildingDefinitions.Pop();

            Debug.Assert(lastest.Equals(source));

            return type;
        });

    }

    public TypeBase CreateReference(TSource source, IMetaProvider<TSource> meta)
    {
        if (_definitions.TryGetValue(source, out var type))
            return TS.CreateReference(type, null);

        foreach (var creator in _pipeline)
        {
            HitTestResult hit = creator.HitTest(source);

            if (hit)
            {
                type = creator.CreateReference(source, meta, hit.State);
                break;
            }
        }

        if (type == null)
            throw new InvalidOperationException($"Unable to create type reference for {Descriptor.Describe(source)}");

        if (!FileFactory.Options.DisableNullable && meta.IsNullable && !meta.IsTypeMember)
        {
            
            type = FileFactory.UseUtility(UtilityFactory.OrNull, type);
        }

        return type;
    }

    public bool IsSourceCreating(TSource source)
    {
        return _buildingDefinitions.Contains(source);
    }

    public bool IsDefined(TSource source)
    {
        return _definitions.ContainsKey(source);
    }

    public bool TryGetDefinition(TSource source, [NotNullWhen(true)]out TypeBase? definition)
    {
        return _definitions.TryGetValue(source, out definition);
    }

    protected virtual bool TryResolveTypeOverrideParameter(object value, IPropertyMetaProvider<TSource> meta, [NotNullWhen(true)] out TypeBase? type)
    {
        if (value is string name)
        {
            type = new IdentifierType(name);
            return true;
        }

        if (value is TypeScriptPrimitive primitive)
        {
            type = TS.Native.Primitive(primitive);
            return true;
        }

        if (value is TSource source)
        {
            type = CreateReference(source, meta);
            return true;
        }

        type = null;
        return false;
    }

    private TypeBase ResolveTypeOverrideParameter(object value, IPropertyMetaProvider<TSource> meta)
    {
        if (TryResolveTypeOverrideParameter(value, meta, out var type))
            return type;

        throw new NotSupportedException("Unsupported type overriden parameter: " + value);
    }

    public TypeBase UseTypeOverride(IPropertyMetaProvider<TSource> meta)
    {
        var ova = meta.TypeOverride;

        if (ova == null ||ova.TypeName == null)
            throw new ArgumentException("Invalid property meta.");

        string? typeName = ova.TypeName, moduleName = ova.ModuleName;
        var typeParameters = ova.TypeParameters;
        TypeBase? type;

        if (moduleName == null)
        {
            if (TryInternalTypeOverride(meta, out type))
                return type;

            if (TS.Native.TryGetExportedType(typeName, out type))
            {
                return TS.CreateReference(type, typeParameters?.Select(x => ResolveTypeOverrideParameter(x, meta)).Cast<TypeBase>().ToList()!);
            }
            else
                throw new CodeException($"TypeScript utility type {typeName} is not found.");

        }

        type = FileFactory.UseShadowType(moduleName, typeName, (name) =>
            new(name, typeParameters?.Length ?? 0));

        return type.Reference(typeParameters?.Select(x => ResolveTypeOverrideParameter(x, meta)).ToArray() ?? Array.Empty<TypeBase>());
    }

    protected virtual bool TryInternalTypeOverride(IPropertyMetaProvider<TSource> meta, [NotNullWhen(true)]out TypeBase? type)
    {
        var typeName = meta.TypeOverride!.TypeName;
        var typeParameters = meta.TypeOverride!.TypeParameters;

        if (Enum.TryParse<TypeScriptPrimitive>(typeName, true, out var primitive))
        {
            type = TS.Native.Primitive(primitive);
            return true;
        }    

        if (typeName == TypeConstants.TypeScriptTuple)
        {
            if (typeParameters == null || typeParameters.Any() == false)
                throw new CodeException("Tuple type requires at least one parameter.");

            type = UseTupleOverride(typeParameters, meta);
            return true;
        }

        type = null;
        return false;
    }

    public TypeBase UseTupleOverride(IEnumerable<object> typeParameters, IPropertyMetaProvider<TSource> meta)
    {
        return new TupleType(typeParameters.Select(x => ResolveTypeOverrideParameter(x, meta)));
    }


    protected delegate bool TryCreateTypeDelegate(TSource source, [NotNullWhen(true)] out TypeBase? type);
}
