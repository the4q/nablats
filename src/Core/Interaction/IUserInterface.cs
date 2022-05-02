using System.Diagnostics.CodeAnalysis;

namespace Nabla.TypeScript.Tool.Interaction
{
    public interface IUserInterface
    {
        void Output(string message, Severity severity, bool newLine);

        [return: NotNullIfNotNull("defaultOption")]
        string? InputString(string prompt, int? defaultOption, params string[] options);

        [return: NotNullIfNotNull("defaultOption")]
        char? InputChar(string prompt, int? defaultOption, bool echo, params char[] options);
    }
}
