namespace Nabla.TypeScript.Tool.Cli;

public class CliOptions
{
    
    public bool DryRun { get; set; }

    public string? Assembly { get; set; }

    public string? Project { get; set; }

    public string? Configuration { get; set; }

    public bool CamelCase { get; set; }

    public string? Output { get; set; }

    public bool JsonNet { get; set; }

    public TypeDiscoveryStrategy Strategy { get; set; }

    public bool Compact { get; set; }

    public bool DoubleQuote { get; set; }

    public FileArrangement Arrange { get; set; }

    public bool Silent { get; set; }

    public string? FileName { get; set; }

    public DateHandling Date { get; set; }

    public EnumHandling Enum { get; set; }

    public string? Header { get; set; }

    public string? Footer { get; set; }

    public int IndentSize { get; set; } = TypeWriterOptions.DefaultIndentSize;

    public bool TabIndent { get; set; }

    public bool Semicolon { get; set; }

    public bool Namespace { get; set; }

    public string? Discoverer { get; set; }

    public bool NoNullable { get; set; }

#if DEBUG
    public bool Debug { get; set; }

    public bool NoBoot { get; set; }


#endif
}
