namespace Nabla.TypeScript.Tool;

public abstract class TypeDefinitionCreator<TSource> : TypeCreator<TSource>
    where TSource : notnull
{
    protected TypeDefinitionCreator(TypeFactory<TSource> factory) : base(factory)
    {
    }

    public TypeBase CreateDefinition(TSource source, object? state)
    {
        if (!Descriptor.IsTypeDefinition(source))
            throw new ArgumentException($"{Descriptor.GetFullName(source)} is not a type definition.");

        return CreateDefinitionCore(source, state);
    }

    protected abstract TypeBase CreateDefinitionCore(TSource source, object? state);

}
