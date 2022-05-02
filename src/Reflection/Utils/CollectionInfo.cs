namespace Nabla.TypeScript.Tool.Reflection;

internal sealed class CollectionInfo : CollectionInfo<Type>
{
    public CollectionInfo(bool isArray, Type elementType)
        : base(isArray, elementType)
    {
    }

}