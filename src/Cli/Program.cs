using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Nabla.TypeScript.Tool.Cli;

static class Program
{
    private const string BootstrapCommandName = "_boot_", DebugCommandName = "_debug_";


    public static Task<int> Main(string[] args)
    {
        if (args.ElementAtOrDefault(0) == DebugCommandName)
        {
            CliOptions options = new()
            {
                Assembly = "..\\..\\..\\..\\Test\\TestModels2\\bin\\Debug\\net6.0\\TestModels2.dll",
                DryRun = true,
                Arrange = FileArrangement.Namespace
            };

            return Task.FromResult((int)GenerateAsync(options).Result);
        }
        else
            return RunCommandAsync(args);
    }



    private static Task<int> RunCommandAsync(string[] args)
    {
        var root = InitializeRootCommand();

        return root.InvokeAsync(args);
    }

    private static Command InitializeRootCommand()
    {

        Option<CliOptions> optionsSymbol = new("options");

        var root = new RootCommand("A dotnet tool for generating TypeScript code from .NET POCO types.")
            .AddOption<string>("project", "Path to the .csproj file. If omitted, will search one in current directory.", 'p')
            .AddOption("configuration", "Project configuration to resolve output assembly.", 'f', getDefault: () => DotNetProject.DefaultConfiguration, argHelp: "name")
            .AddOption<string>("assembly", "Path to the .NET assembly to discover source types. If set, will override the --project option.", 'a', argHelp: "file_path")
            .AddOption<string>("output", "Path to the directory where to store generated TypeScript files.", 'o', argHelp: "dir_path")
            .AddOption<TypeDiscoveryStrategy>("strategy", "The way to discovery POCO types", 's', getDefault: () => default)
            .AddOption<FileArrangement>("arrange", "How the generated TypeScript modules should be arranged.", 'r', getDefault: () => default)
            .AddOption<string>("file-name", "Default file name for Explicit or Single arrangement.", 'm', argHelp: "name")
            .AddOption<bool>("namespace", "Generate TypeScript code with namespaces.", 'n')
            .AddOption<string>("discoverer", "Full CLR name of type to use for discovering POCO types.", 'S', argHelp: "type_name")
            .AddOption<bool>("camel-case", "Use camel-case for type members.", 'c')
            .AddOption<DateHandling>("date", "How date types be mapped.", 'd', getDefault: () => default)
            .AddOption<EnumHandling>("enum", "How enum types be mapped.", 'e', getDefault: () => default)
            .AddOption<bool>("no-nullable", "Disable nullables", 'u')
            .AddOption<bool>("compact", "Compact output file by eliminating blank lines.", 'P')
            .AddOption<bool>("double-quote", "Use double-quote for string literals.", 'D')
            .AddOption("indent-size", "Indent size, will ignored when using tab indent.", 'I', getDefault: () => TypeWriterOptions.DefaultIndentSize, argHelp: "num_of_space")
            .AddOption<bool>("tab-indent", "Use \\t instead of spaces.", 'T')
            .AddOption<bool>("semicolon", "Add semicolons if necessary.", 'C')
            .AddOption<string>("header", "Path to a file which you want its content been embeded into top of each output file.", argHelp: "file_path")
            .AddOption<string>("footer", "Path to a file which you want its content been embeded into bottom of each output file.", argHelp: "file_path")
            .AddOption<bool>("watch", "Watches target assembly and update output when changed automatically.", 'w')
            .AddOption<bool>("dry-run", "Print generated code on console, do not write any file.")
            .AddOption<bool>("silent", "Do not output any information except generated code with --dry-run option set.")
#if DEBUG
            .AddOption<bool>("debug", "Run this tool in debug mode.")
            .AddOption<bool>("no-boot", "Do not use bootstrap. May cause assembly fail to load.", addGlobally: false)
#endif
            ;

        root.Handler = CommandHandler.Create(BootstrapAsync);

        Command cmd = new(BootstrapCommandName);
        cmd.IsHidden = true;
        cmd.Handler = CommandHandler.Create(GenerateAsync);
        root.AddCommand(cmd);

        return root;
    }

    private static async Task<int> BootstrapAsync(CliOptions options, bool watch)
    {
#if DEBUG
        if (options.NoBoot && !watch)
        {
            return (int)await GenerateAsync(options);
        }
#endif

        var ret = await RunProcessAsync(options);

        if (watch)
        {
            if (ret == (int)AppReturnCode.Success)
            {
                return await WatchAsync(options);
            }
            else
            {
                UI.Warning("Cannot start watching due to first generating error.");
            }
        }    
        
        return ret;
    }

    internal static async Task<int> RunProcessAsync(CliOptions options)
    {
        FileInfo file;

        if (options.Assembly == null)
        {
            if (!DotNetProject.EvaluateTargetPath(options.Project, options.Configuration, out var path, out var error))
            {
                UI.Error(error);
                return (int)AppReturnCode.EvaluateProjectFileFailed;
            }

            file = new(path);
        }
        else
            file = new(options.Assembly);

        if (file.Exists)
        {
            var runtimeconfig = Path.Combine(file.DirectoryName!, Path.GetFileNameWithoutExtension(file.Name) + ".runtimeconfig.json");
            ProcessStartInfo startInfo = new("dotnet");
            var args = startInfo.ArgumentList;

            args.Add("exec");
            if (File.Exists(runtimeconfig))
            {
                args.Add("--runtimeconfig");
                args.Add(runtimeconfig);
            }


            string[] cmdArgs = Environment.GetCommandLineArgs();
            args.Add(cmdArgs[0]);
            args.Add(BootstrapCommandName);

            foreach (var arg in cmdArgs.Skip(1))
                args.Add(arg);

            var process = Process.Start(startInfo);

            if (process != null)
            {
                await process.WaitForExitAsync();

                return process.ExitCode;
            }
            else
            {
                UI.Error("Unable to boot up generating process.");
                return (int)AppReturnCode.BootstrapFailed;
            }
        }
        else
            return (int)AppReturnCode.AssemblyNotFound;
    }

    private static async Task<AppReturnCode> GenerateAsync(CliOptions options)
    {
        UI.Silent = options.Silent;
#if DEBUG
        UI.DebugMode = options.Debug;
#endif

        TypeScriptGenerator generator = new(options);

        try
        {
            return await generator.RunAsync();
        }
#if !DEBUG
        catch (CodeException ex)
        {
            UI.Error(ex.Message);
            return AppReturnCode.CodeError;
        }
#endif
        catch (Exception ex)
        {
            UI.Error("Unhandled error occured.\r\n" + ex);
            return AppReturnCode.UnhandledError;
        }
    }

    private static async Task<int> WatchAsync(CliOptions options)
    {
        using var watcher = new AssemblyWatcher(options);
        await watcher.Start();
        return watcher.LastExitCode;
    }

}
