namespace Nabla.TypeScript.Tool.Reflection;

internal sealed class BaseTypeSite : ISite
{
    public BaseTypeSite(Type subclass)
    {
        Subclass = subclass;
    }

    public Type Subclass { get; }

    public bool IsNullable => false;

    public bool IsTypeMember => false;

    Type ISite.ReferencingType => Subclass;
}
