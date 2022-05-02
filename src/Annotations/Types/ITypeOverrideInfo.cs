
namespace Nabla.TypeScript.Tool
{
    public interface ITypeOverrideInfo
    {
        int ArrayDepth { get; }

        string? ModuleName { get; }
        
        string TypeName { get; }

        object[]? TypeParameters { get; }
    }
}