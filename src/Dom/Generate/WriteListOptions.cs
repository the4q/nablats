namespace Nabla.TypeScript;

public sealed class WriteListOptions
{
    public static readonly WriteListOptions Default = new()
    {

    };

    public static readonly WriteListOptions Union = new()
    {
        Separator = '|',
        InsertSpaceBeforeSeparator = true
    };

    public static readonly WriteListOptions ImportMembers = new()
    {
        SurroundWith = SurroundingChar.Brace,
        Padding = true,
    };

    public static readonly WriteListOptions GenericParameters = new()
    {
        SurroundWith = SurroundingChar.Chevron
    };

    public static readonly WriteListOptions BlockBody = new()
    {
        LineControl = LineControl.Multiple
    };

    public static readonly WriteListOptions BlockBetween = new()
    {
        LineControl = LineControl.ExtraBlankLine,
        Separator = default
    };

    public static readonly WriteListOptions Statements = new()
    {
        LineControl = LineControl.Multiple,
        Separator = default
    };

    public static readonly WriteListOptions Array = new()
    {
        SurroundWith = SurroundingChar.Bracket,
    };

    public bool Padding { get; init; }

    public bool Multiline => LineControl != LineControl.Single;

    public LineControl LineControl { get; init; }

    public bool InsertSpaceBeforeSeparator { get; init; }

    public SurroundingChar? SurroundWith { get; init; }

    public char Separator { get; init; } = ',';

    
}

public enum LineControl
{
    Single,
    Multiple,
    ExtraBlankLine
}