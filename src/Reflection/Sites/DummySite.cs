namespace Nabla.TypeScript.Tool.Reflection;

internal sealed class DummySite : DummyMetaProvider<Type>, ISite
{
    public Type ReferencingType => Source;

    public DummySite(Type referencingType)
        : base(referencingType)
    {
    }
}