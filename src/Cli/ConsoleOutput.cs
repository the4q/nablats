namespace Nabla.TypeScript.Tool.Cli;

internal sealed class ConsoleOutput : CodeOutput
{
    public ConsoleOutput(CodeOutputOptions options) : base(options)
    {
    }

    public override bool ShouldDisposeTextWriter => false;

    public override Task<TextWriter> BeginFileAsync(string filename, TypeFile module)
    {
        UI.Comment($"/* {filename} */");

        return Task.FromResult(Console.Out);
    }

    public override Task CompleteAsync()
    {
        return Task.CompletedTask;
    }

    public override Task EndFileAsync(TypeWriter writer, string filename)
    {
        return Task.CompletedTask;
    }

    public override Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}