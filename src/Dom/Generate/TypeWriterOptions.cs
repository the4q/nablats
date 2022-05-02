namespace Nabla.TypeScript;

public sealed class TypeWriterOptions
{
    public const int DefaultIndentSize = 4;

    public bool UseTabIndent { get; set; }

    public int IndentSize { get; set; } = DefaultIndentSize;

    public bool UseDoubleQuote { get; set; }

    public bool InsertBlankLine { get; set; } = true;

    public bool UseSemicolon { get; set; }

}