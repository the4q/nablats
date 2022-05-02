namespace Nabla.TypeScript.Tool;

public class TypeParameterCreator<TSource> : TypeCreator<TSource>
    where TSource : notnull
{
    public TypeParameterCreator(TypeFactory<TSource> factory) : base(factory)
    {
    }

    public override HitTestResult HitTest(TSource source)
    {
        return new(Descriptor.IsTypeParameter(source));
    }

    public override TypeBase CreateReference(TSource source, IMetaProvider<TSource> meta, object? state)
    {
        return TS.Parameter(Descriptor.GetName(source)).Reference();
    }
}
