using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Nabla.TypeScript.Tool;

public sealed class FileSystemOutput : CodeOutput
{

    private string _outputDir = null!, _hashPath = null!;
    private Dictionary<string, string> _hashes = null!;
    private WriteFileState? _state;

    public FileSystemOutput(CodeOutputOptions options)
        : base(options)
    {

    }

    private static string InitializeStateStore()
    {
        string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "nablats");

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        return dir;
    }

    public override async Task InitializeAsync()
    {
        string? outputDir = Options.OutputDir;

        if (outputDir != null)
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
        }

        _outputDir = string.IsNullOrEmpty(outputDir) ? Environment.CurrentDirectory : Path.GetFullPath(outputDir);

        _hashPath = Path.Combine(InitializeStateStore(), "hashes");

        if (File.Exists(_hashPath))
        {
            try
            {
                using var stream = File.OpenRead(_hashPath);
                _hashes = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream) ?? new();
            }
            catch
            {

            }
        }
        
        _hashes ??= new();

    }

    public override Task<TextWriter> BeginFileAsync(string filename, TypeFile module)
    {
        Debug.Assert(_state == null);

        string path = Path.Combine(_outputDir, filename);

        _state = new(Path.GetTempFileName(), path);

        FileInfo fileInfo = new(_state.TempPath);

        UI.Say(_state.OutputPath, newLine: false);

        var stream = fileInfo.Open(FileMode.Create, FileAccess.ReadWrite);

        return Task.FromResult((TextWriter)new StreamWriter(stream));
    }

    public override async Task EndFileAsync(TypeWriter writer, string filename)
    {
        Debug.Assert(_state != null);

        var state = _state;
        var stream1 = ((StreamWriter)writer.InnerWriter).BaseStream;
        string? hash0, hash1, hashKey = Path.GetFullPath(state.OutputPath), status = null;

        await writer.InnerWriter.FlushAsync();
        await stream1.FlushAsync();

        stream1.Position = 0;
        hash1 = await HashFile(stream1);

        if (File.Exists(state.OutputPath))
        {
            hash0 = await HashFile(state.OutputPath);

            if (_hashes.TryGetValue(hashKey, out var hastLatest) && hash0 != hastLatest)
            {
                if (!UI.Silent)
                {
                    UI.NewLine();
                    if (!UI.Confirm($"The output file `{state.OutputPath}` has changed since last generating, overwrite it?"))
                        hash0 = hash1;
                    status = string.Empty;
                }
                
            }
        }
        else
        {
            hash0 = null;
            status = "Created";
        }

        if (hash0 != hash1)
        {
            using var stream0 = File.Create(state.OutputPath);
            stream1.Position = 0;

            await stream1.CopyToAsync(stream0);

            if (status == null)
                status = "Overwrited";
        }
        else if (status == null)
            status = "Unchanged";

        stream1.Close();
        File.Delete(state.TempPath);

        _hashes[hashKey] = hash1;
        _state = null;

        if (status != string.Empty)
            UI.Say(status != null ? $" ... [{status}]" : string.Empty);
    }

    public override async Task CompleteAsync()
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
        };


        using var writer = File.Create(_hashPath);
        await JsonSerializer.SerializeAsync(writer, _hashes, options);
    }

    // Handling output file disposition in EndFileAsync method.
    public override bool ShouldDisposeTextWriter => false;

    private static async Task<string?> HashFile(string path)
    {
        if (File.Exists(path))
        {
            using FileStream stream = File.OpenRead(path);
            return await HashFile(stream);
        }

        return null;
    }

    private static async Task<string> HashFile(Stream stream)
    {
        using var md5 = MD5.Create();

        var data = await md5.ComputeHashAsync(stream);

        StringBuilder hash = new(data.Length * 2);

        for (int i = 0; i < data.Length; i++)
        {
            byte b = data[i];

            hash.Append(b.ToString("x2"));
        }

        return hash.ToString();
    }

    record WriteFileState(string TempPath, string OutputPath);
}

