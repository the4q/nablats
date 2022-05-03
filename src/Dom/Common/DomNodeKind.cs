namespace Nabla.TypeScript;

public enum DomNodeKind
{
    TypeFile,
    TypeDeclaration,
    NamespaceDeclaration,
    NodeReference = 10,
    TypeReference,
    ModuleImport,
    NamespaceImport,
    TypeDefinition = 20,
    ClassDefinition,
    Property = 30,
    Parameter,
    Identifier,
    Unknown = 50
}