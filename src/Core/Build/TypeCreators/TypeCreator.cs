namespace Nabla.TypeScript.Tool;

public abstract class TypeCreator<TSource>
    where TSource : notnull
{
    public TypeCreator(TypeFactory<TSource> factory)
    {
        Factory = factory;
    }

    public TypeFactory<TSource> Factory { get; }

    public ISourceDescriptor<TSource> Descriptor => Factory.Descriptor;

    public abstract HitTestResult HitTest(TSource source);

    public abstract TypeBase CreateReference(TSource source, IMetaProvider<TSource> meta, object? state);

}
