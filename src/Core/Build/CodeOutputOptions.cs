namespace Nabla.TypeScript.Tool;

public class CodeOutputOptions
{
    public string? OutputDir { get; set; } 

    //public bool InsertBlankLine { get; set; } = true;

    //public bool UseDoubleQuote { get; set; }

    //public int IndentSize { get; set; } = 4;

    //public bool UseTabIndent { get; set; }

    public bool? EnforceDateType { get; set; }

    public bool GenerateComments { get; set; }

    //public PropertyNamingPolicy NamingPolicy { get; set; } = PropertyNamingPolicy.CamelCase;

    public string? FileHeaderPath { get; set; }

    public string? FileFooterPath { get; set; }

    public TypeWriterOptions? WriterOptions { get; set; }
}
