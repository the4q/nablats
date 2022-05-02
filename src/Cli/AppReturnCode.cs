namespace Nabla.TypeScript.Tool.Cli;

public enum AppReturnCode
{
    Success,
    WrongParameter,
    AssemblyNotFound,
    AssemblyLoadFailed,
    NotPocoTypeFound,
    BootstrapFailed,
    UnhandledError,
    CodeError,
    EvaluateProjectFileFailed
}
