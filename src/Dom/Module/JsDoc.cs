namespace Nabla.TypeScript;

public class JsDoc
{
    private IDictionary<string, string>? _parameters;

    public string? Remarks { get; set; }

    public string? Returns { get; set; }

    public bool IsEmpty => string.IsNullOrEmpty(Returns) && string.IsNullOrEmpty(Remarks) && _parameters?.Any() == false;

    public IDictionary<string, string> Parameters
    {
        get
        {
            if (_parameters == null)
                _parameters = new Dictionary<string, string>();

            return _parameters;
        }
        set => _parameters = value;
    }

    public void Write(TypeWriter writer)
    {
        writer.EnsureNewLine();

        writer.WriteLine("/**");

        if (Remarks != null)
        {
            Write(writer, Remarks, null, null);
        }

        if (_parameters != null)
        {
            foreach (var item in _parameters)
            {
                Write(writer, item.Value, "param", item.Key);
            }
        }

        if (Returns != null)
            Write(writer, Returns, "returns", null);

        writer.WriteLine(" */");
    }

    

    private static void Write(TypeWriter writer, string content, string? directive, string? param)
    {
        writer.WriteSpace().Write('*').WriteSpace();

        if (directive != null)
        {
            writer.Write('@').Write(directive).WriteSpace();
            if (param != null)
                writer.Write(param).WriteSpace();
        }

        var lines = content.ToLines();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (i > 0)
                writer.WriteSpace().Write('*').WriteSpace();
            writer.WriteLine(line);
        }
    }
}
