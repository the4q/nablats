namespace Nabla.TypeScript.Tool;

public class CollectionInfo<TSource>
    where TSource: notnull
{
    public CollectionInfo(bool isArray, TSource elementType)
    {
        IsArray = isArray;
        ElementType = elementType;
    }

    public bool IsArray { get; }

    public TSource ElementType { get; }
}