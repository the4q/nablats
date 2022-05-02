using System.CommandLine;

namespace Nabla.TypeScript.Tool.Cli;

internal static class CommandConfigurationExtensions
{
    public static Command AddOption<T>(this Command command,
                                       string name,
                                       string description,
                                       char? alias = null,
                                       bool isRequired = false,
                                       Func<T>? getDefault = null,
                                       bool addGlobally = true,
                                       string? argHelp = null,
                                       Action<Option<T>>? configure = null)
    {
        List<string> names = new(2);

        if (alias != null)
            names.Add($"-{alias}");

        names.Add($"--{name}");

        var option = new Option<T>(names.ToArray(), description)
        {
            IsRequired = isRequired,
            ArgumentHelpName = argHelp,
        };

        if (getDefault != null)
        {
            option.SetDefaultValueFactory(() => getDefault());
        }

        configure?.Invoke(option);

        if (addGlobally)
            command.AddGlobalOption(option);    
        else
            command.AddOption(option);

        return command;
    }

}