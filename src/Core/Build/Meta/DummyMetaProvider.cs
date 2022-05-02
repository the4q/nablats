namespace Nabla.TypeScript.Tool;

public class DummyMetaProvider<TSource> : IMetaProvider<TSource>
    where TSource : notnull
{
    public DummyMetaProvider(TSource source)
    {
        Source = source;
    }

    public TSource Source { get; }

    public bool IsNullable => false;

    public bool IsTypeMember => false;
}