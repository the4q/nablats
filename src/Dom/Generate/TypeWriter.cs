namespace Nabla.TypeScript;

public sealed class TypeWriter : IDisposable
{
    private readonly char[][] _surroundingChars = new char[][] {
        new char[] { '(', ')' },
        new char[] { '[', ']' },
        new char[] { '{', '}' },
        new char[] { '<', '>' },
    };

    private readonly TextWriter _writer;
    private readonly bool _disposeWriter;
    private int _indentLevel, _charPos;

    public TypeWriter(TextWriter writer, TypeWriterOptions options, bool disposeWriter)
    {
        _writer = writer;
        Options = options;
        _disposeWriter = disposeWriter;
    }

    public bool IsBeginLine => _charPos == 0;

    public TextWriter InnerWriter => _writer;

    public TypeWriterOptions Options { get; }

    private TypeWriter WriteIndent()
    {
        if (_indentLevel < 1)
            return this;

        string indent;

        if (Options.UseTabIndent)
        {
            indent = new('\t', _indentLevel);
        }
        else
        {
            int count = Options.IndentSize * _indentLevel;
            indent = new(' ', count);
        }

        _writer.Write(indent);
        return this;
    }

    public TypeWriter Write(char c)
    {
        if (c >= ' ')
        {
            if (IsBeginLine && _indentLevel > 0)
            {
                WriteIndent();
            }

            _writer.Write(c);
            _charPos++;
        }

        return this;
    }

    public TypeWriter WriteSpace(int count = 1)
    {
        for (int i = 0; i < count; i++)
            Write(' ');

        return this;
    }

    public TypeWriter Write(string content)
    {
        foreach (var ch in content)
            Write(ch);

        return this;
    }

    public TypeWriter WriteLine(string? content = null)
    {
        if (!string.IsNullOrEmpty(content))
            Write(content);

        _writer.WriteLine();
        _charPos = 0;

        return this;
    }

    public TypeWriter BeginBlock()
    {
        Write('{')
            .WriteLine();
        _indentLevel++;

        return this;
    }

    public TypeWriter EndBlock()
    {
        _indentLevel--;

        return Write('}').WriteLine();
    }

    public TypeWriter WriteList<T>(IEnumerable<T> items, Action<T> write, WriteListOptions? options = null)
    {
        bool comma = false;

        options ??= WriteListOptions.Default;

        if (options.SurroundWith != null)
        {
            Write(_surroundingChars[(int)options.SurroundWith.Value][0]);

            if (!options.Multiline && options.Padding)
                WriteSpace();
        }

        foreach (var item in items)
        {
            if (comma)
            {
                if (options.InsertSpaceBeforeSeparator && !options.Multiline)
                    WriteSpace();

                Write(options.Separator);

                if (options.Multiline)
                {
                    EnsureNewLine();
                    if (options.LineControl == LineControl.ExtraBlankLine)
                        InsertBlankLine();
                }
                else
                    WriteSpace();
            }
            else
                comma = true;

            write(item);

        }

        if (options.Multiline)
            WriteLine();

        if (options.SurroundWith != null)
        {
            if (!options.Multiline && options.Padding)
                WriteSpace();
            Write(_surroundingChars[(int)options.SurroundWith.Value][1]);
        }

        return this;
    }

    public TypeWriter WriteList<T>(IEnumerable<T> items, WriteListOptions? options = null)
        where T : DomNode => WriteList(items, item => item.Write(this), options);

    public TypeWriter WriteGenericParameters(IEnumerable<TypeParameter> parameters)
    {
        if (parameters.Any())
            WriteList(parameters, WriteListOptions.GenericParameters);
        return this;
    }

    public TypeWriter WriteLiteral(string value)
    {
        char mark = Options.UseDoubleQuote ? '"' : '\'';

        Write(mark);

        foreach (var ch in value)
        {
            if (ch == mark || ch == '\\')
            {
                Write('\\');
            }

            Write(ch);
        }

        return Write(mark);
    }

    public TypeWriter WriteSemicolon()
    {
        if (Options.UseSemicolon)
            Write(';');

        return this;
    }

    public TypeWriter WriteAttribute(string name, string? value)
    {
        Write(name);

        if (value != null)
        {
            Write('=')
                .WriteAttributeValue(value);
        }

        return this;
    }

    public TypeWriter WriteAttributeValue(string value)
    {
        Write('"');

        foreach (var ch in value)
        {
            if (char.IsControl(ch) || ch == '"')
            {
                Write("&#x").Write(((int)ch).ToString("X2")).Write(';');
            }
            else
                Write(ch);
        }

        return Write('"');
    }

    public TypeWriter WriteLineComment(string comment, bool insertSpace = true)
    {
        if (!IsBeginLine)
            WriteLine();

        foreach (var line in comment.ToLines())
        {
            Write("//");
            if (insertSpace)
                WriteSpace();

            WriteLine(line);
        }

        return this;
    }

 

    public TypeWriter InsertBlankLine(int count = 1)
    {
        if (Options.InsertBlankLine)
        {
            if (!IsBeginLine)
                WriteLine();

            for (int i = 0; i < count; i++)
                WriteLine();
        }

        return this;
    }

    //public TypeWriter WriteTypeName(TypescriptType type)
    //{
    //    Write(type.Name);

    //    if (type.Parameters.Any())
    //    {
    //        WriteList(type.Parameters, pt => WriteTypeName(pt), WriteListOptions.GenericParameters);
    //    }

    //    return this;
    //}

    public TypeWriter WriteNode(DomNode? node)
    {
        node?.Write(this);
        return this;
    }

    public TypeWriter EnsureNewLine()
    {
        if (!IsBeginLine)
            WriteLine();

        return this;
    }

    public void Dispose()
    {
        if (_disposeWriter)
            ((IDisposable)_writer).Dispose();
    }
}
