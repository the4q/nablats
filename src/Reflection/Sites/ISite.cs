namespace Nabla.TypeScript.Tool.Reflection;

internal interface ISite : IMetaProvider<Type>
{
    Type ReferencingType { get; }

    Type IMetaProvider<Type>.Source => ReferencingType;
}
