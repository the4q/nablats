namespace Nabla.TypeScript.Tool;

public interface IMetaProvider<TSource>
    where TSource : notnull
{
    bool IsNullable { get; }

    bool IsTypeMember { get; }

    TSource Source { get; }
}

