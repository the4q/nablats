using Nabla.TypeScript.Tool.Reflection;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Nabla.TypeScript.Tool.Cli;

internal class TypeScriptGenerator
{
    public TypeScriptGenerator(CliOptions options)
    {
        Options = options;
    }

    public CliOptions Options { get; }

    private static ISerializationInfo CreateSerializationInfo(CliOptions options)
    {
        if (options.JsonNet)
            throw new NotSupportedException("Newtonsoft JSON.NET is not supported.");

        return new SystemTextJsonSerializationInfo();
    }

    private static FileOrganizer CreateFileCoordinator(CliOptions options)
    {
        FileOrganizer resolver = options.Arrange switch
        {
            FileArrangement.Nature => new NatureFileResolver(new AssemblyModuleNameResolver()),
            FileArrangement.Single => new SingleFileResolver(new AssemblyModuleNameResolver()),
            FileArrangement.Namespace => new NatureFileResolver(new NamespaceModuleNameResolver()),
            _ => new ExplicitFileResolver(new AssemblyModuleNameResolver()),
        };

        resolver.DefaultFileName = options.FileName;

        return resolver;
    }

    private bool TryLoadAssembly([NotNullWhen(true)]out Assembly? assembly, out AppReturnCode code)
    {
        string? path = Options.Assembly;

        if (string.IsNullOrEmpty(path))
        {
            if (!DotNetProject.EvaluateTargetPath(Options.Project, Options.Configuration, out path, out var error))
            {
                UI.Error(error);
                code = AppReturnCode.EvaluateProjectFileFailed;
                assembly = null;
                return false;
            }
        }

        FileInfo assemblyFile = new(path);

        if (!assemblyFile.Exists)
        {
            UI.Error($"Assembly file {assemblyFile.FullName} not found.");
            code = AppReturnCode.AssemblyNotFound;
            assembly = null;
            return false;
        }

        try
        {
            assembly = Assembly.LoadFrom(assemblyFile.FullName);
        }
        catch (Exception ex)
        {
            UI.Error($"Assembly load failed: {ex.Message}");
            code = AppReturnCode.AssemblyLoadFailed;
            assembly = null;
            return false;
        }

        code = AppReturnCode.Success;
        return true;
    }

    public async Task<AppReturnCode> RunAsync()
    {
        if (!TryLoadAssembly(out var assembly, out var code))
            return code;

        FileFactory factory = new ReflectionFileFactory(assembly,
                                                        CreateSerializationInfo(Options),
                                                        CreateFileCoordinator(Options),
                                                        new()
                                                        {
                                                            DateHandling = Options.Date,
                                                            NamingPolicy = Options.CamelCase ? PropertyNamingPolicy.CamelCase : PropertyNamingPolicy.Unchanged,
                                                            UseNamespaces = Options.Namespace,
                                                            EnumHandling = Options.Enum,
                                                            DisableNullable = Options.NoNullable
                                                        },
                                                        Options.Strategy)
        {
            DiscovererTypeName = Options.Discoverer
        };

        var stopWatch = Stopwatch.StartNew();

        UI.Info("Start discovering .NET types and creating TypeScript files.");
        UI.Say("Assembly: " + assembly.Location);

        var modules = factory.CreateFiles();

        if (modules.Any())
        {
            FileBuilder builder = new(CreateOutput());
            
            UI.Info($"Begin generating TypeScript code");

            await builder.BuildAsync(modules);

            int moduleCount = modules.Count, typeCount = modules.SelectMany(x => x.AllDeclarations).Count();

            UI.Success($"Total {typeCount} type(s) of {moduleCount} module(s) generated in {stopWatch.Elapsed.TotalSeconds:0.##}s.");

            return AppReturnCode.Success;
        }
        else
        {
            UI.Warning("No TypeScript module found.");
            return AppReturnCode.NotPocoTypeFound;
        }
    }

    private CodeOutput CreateOutput()
    {
        CodeOutputOptions options = new()
        {
            OutputDir = Options.Output,
            FileHeaderPath = Options.Header,
            FileFooterPath = Options.Footer,
            WriterOptions = new()
            {
                InsertBlankLine = !Options.Compact,
                UseDoubleQuote = Options.DoubleQuote,
                IndentSize = Options.IndentSize,
                UseTabIndent = Options.TabIndent,
                UseSemicolon = Options.Semicolon,
            }
        };


        if (Options.DryRun)
            return new ConsoleOutput(options);
        else
            return new FileSystemOutput(options);
    }


}
