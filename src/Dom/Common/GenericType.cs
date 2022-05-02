namespace Nabla.TypeScript;

public abstract class GenericType : TypeBase
{
    public GenericType(IEnumerable<TypeParameter>? parameters)
    {
        GenericParameters = new(this, parameters ?? Array.Empty<TypeParameter>());
    }

    public NamedDomNodeCollection<TypeParameter> GenericParameters { get; }

    //public GenericReference CreateReference(params TypescriptType[] arguments)
    //{
    //    return new(this, arguments);
    //}

    //public TypeReference CreateReference(IEnumerable<TypescriptType> arguments)
    //    => CreateReference(arguments.ToArray());

}
