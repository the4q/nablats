namespace Nabla.TypeScript.Tool;

public interface IFileNameResolver
{
    string ResolveFileName(object source);

    string? ResolveNamespace(object source);
}