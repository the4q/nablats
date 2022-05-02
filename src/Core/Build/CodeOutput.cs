namespace Nabla.TypeScript.Tool;

public abstract class CodeOutput
{
    public CodeOutput(CodeOutputOptions options)
    {
        Options = options;
    }

    public CodeOutputOptions Options { get; }

    public abstract Task InitializeAsync();

    public abstract Task<TextWriter> BeginFileAsync(string filename, TypeFile module);

    public abstract bool ShouldDisposeTextWriter { get; }

    public abstract Task EndFileAsync(TypeWriter writer, string filename);

    public abstract Task CompleteAsync();
}
