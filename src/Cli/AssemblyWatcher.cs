
namespace Nabla.TypeScript.Tool.Cli;

internal sealed class AssemblyWatcher : IDisposable
{
    private readonly CliOptions _options;
    private FileSystemWatcher? _watcher;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _running;
    private int _draw;

    public AssemblyWatcher(CliOptions options)
    {
        _options = options;
        
    }

    public int LastExitCode { get; private set; }

    public Task Start()
    {
        ResetToken();

        _watcher = new FileSystemWatcher(Path.GetDirectoryName(_options.Assembly)!, Path.GetFileName(_options.Assembly)!);

        _watcher.Changed += OnAssemblyChanged;
        _watcher.EnableRaisingEvents = true;


        _running = true;

        UI.Say("Start wathcing assembly for changes, press Ctrl+C to exit.");

        return Loop();
    }

    private void OnControlC(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;

        Stop();
    }

    private void Stop()
    {
        _running = false;
        _cancellationTokenSource?.Cancel();
    }

    private void OnAssemblyChanged(object sender, FileSystemEventArgs e)
    {
        if (!e.ChangeType.HasFlag(WatcherChangeTypes.Deleted))
        {
            int draw = ++_draw;

            Task.Delay(1000)
                .ContinueWith(_ =>
                {
                    if (draw == _draw)
                    {
                        ResetToken();
                    }
                });
        }
    }

    private void ResetToken()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = new();
    }

    private async Task Loop()
    {
        Console.CancelKeyPress += OnControlC;

        while (_running)
        {
            var source = _cancellationTokenSource;

            if (source != null)
            {
                try
                {
                    await Task.Delay(10000, source.Token);
                }
                catch(TaskCanceledException)
                {
                }

                if (source.IsCancellationRequested && _running)
                {
                    UI.Say("Assembly change detected, updating output.");

                    LastExitCode = await Program.RunProcessAsync(_options);

                    if (LastExitCode == (int)AppReturnCode.UnhandledError)
                    {
                        UI.Warning("Watching process terminated due to unhandled error.");
                        Stop();
                    }

                    
                }
            }
            else
                await Task.Delay(1000);
        }
        
        Console.CancelKeyPress -= OnControlC;
    }

    public void Dispose()
    {
        _watcher?.Dispose();
        _cancellationTokenSource?.Dispose();
    }
}