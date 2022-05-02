using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nabla.TypeScript.Tool;

public class FileBuilder
{
    private readonly CodeOutput _output;

    public FileBuilder(CodeOutput output)
    {
        _output = output;
    }

    public async Task BuildAsync(IEnumerable<TypeFile> modules)
    {
        await _output.InitializeAsync();

        foreach (var module in modules)
        {
            await BuildAsync(module);
        }

        await _output.CompleteAsync();
    }

    private async Task BuildAsync(TypeFile module)
    {
        string filename = module.Name + TypeFile.Extension;
        var options = _output.Options;

        using TypeWriter writer = new(await _output.BeginFileAsync(filename, module),
            options.WriterOptions ?? new(),
            _output.ShouldDisposeTextWriter);

        var variables = CreateVariables(module);

        await WriteFileAsync(writer, options.FileHeaderPath, variables);
        writer.WriteNode(module);
        await WriteFileAsync(writer, options.FileFooterPath, variables);

        await _output.EndFileAsync(writer, filename);
    }

    private static readonly Regex _variableRegex = new(@"\$\{([^\}]+)\}", RegexOptions.Compiled);

    private static async Task WriteFileAsync(TypeWriter writer, string? path, Dictionary<string, string> variables)
    {
        if (path != null && File.Exists(path))
        {
            using var input = File.OpenText(path);

            while(true)
            {
                var line = await input.ReadLineAsync();

                if (line == null)
                    break;

                line = _variableRegex.Replace(line, m =>
                {
                    var key = m.Groups[1].Value;
                    if (variables.TryGetValue(key, out var value))
                        return value;

                    if (key.StartsWith("env_"))
                    {
                        var env = key[4..];
                        env = Environment.GetEnvironmentVariable(env);

                        if (env != null)
                            return env;
                    }

                    return m.Value;
                });

                await writer.InnerWriter.WriteLineAsync(line);
            }
        }
    }

    private Dictionary<string, string> CreateVariables(TypeFile file)
    {
        string fileName = file.Name + TypeFile.Extension,
            moduleName = file.Name,
            outputDir = _output.Options.OutputDir == null ? Environment.CurrentDirectory : Path.GetFullPath(_output.Options.OutputDir);

        DateTime now = DateTime.Now;

        
        
        return new()
        {
            ["fileName"] = fileName,
            ["filePath"] = Path.Combine(outputDir, fileName),
            ["moduleName"] = moduleName,
            ["outputPath"] = outputDir,
            ["outputDir"] = Path.GetFileName(outputDir),
            ["date"] = now.ToShortDateString(),
            ["time"] = now.ToLongTimeString(),
            ["dateTime"] = now.ToString("G"),
            ["user"] = Environment.UserName,
            ["machine"] = Environment.MachineName,
            ["version"] = typeof(FileFactory).Assembly.GetName().Version?.ToString(3) ?? string.Empty
        };
    }
}
