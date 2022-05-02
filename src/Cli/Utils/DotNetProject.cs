using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Nabla.TypeScript.Tool;

internal class DotNetProject
{
    public const string DefaultConfiguration = "Debug";

    public DotNetProject(string projectPath)
    {
        ProjectPath = projectPath;
    }

    public string ProjectPath { get; }

    public string? TargetFramework { get; set; }

    public string? AssemblyName { get; set; }

    public string? OutDir { get; set; }

    public string? OutputPath { get; set; }

    public string? OutputType { get; set; }

    public string? Sdk { get; set; }

    public string? GetTargetPath(string? configuration)
    {
        string? path = Path.GetDirectoryName(ProjectPath);

        if (path != null && TargetFramework != null)
        {
            string? outDir = OutDir;

            configuration ??= DefaultConfiguration;

            if (outDir == null)
            {
                if (OutputPath == null)
                    outDir = Path.Combine("bin", configuration, TargetFramework);
                else
                    outDir = Path.Combine(OutputPath, TargetFramework);
            }

            return Path.Combine(path, outDir, GetTargetName()).Replace("$(Configuration)", configuration);
        }

        return null;
    }

    public string GetTargetName()
    {
        var name = AssemblyName ?? Path.GetFileNameWithoutExtension(ProjectPath);

        return name + ".dll"; // GetOutputExtension();
    }

    public string GetOutputExtension()
    {
        if (OutputType == null || OutputType == "Library")
            return ".dll";

        if (OutputType == "WinExe" || Environment.OSVersion.Platform == PlatformID.Win32NT)
            return ".exe";

        return string.Empty;
    }

    public static bool TryFind(string dir, [NotNullWhen(true)]out DotNetProject? project, [NotNullWhen(false)]out string? errorMessage)
    {
        if (!Directory.Exists(dir))
        {
            project = null;
            errorMessage = $"Directory {dir} not exists.";
            return false;
        }

        var files = Directory.GetFiles(dir, "*.csproj");

        if (files.Length == 0)
        {
            project = null;
            errorMessage = $"No project file found in {dir}.";
            return false;
        }

        if (files.Length > 1)
        {
            project = null;
            errorMessage = $"More than one project files found in {dir}.";
            return false;
        }

        return TryParse(files[0], out project, out errorMessage);
    }

    public static DotNetProject Parse(string path)
    {
        XDocument doc = XDocument.Load(path);

        if (doc.Root?.Name == "Project")
        {
            var sdk = (string?)doc.Root.Attribute("Sdk");

            if (sdk != null)
            {
                DotNetProject project = new(path)
                {
                    Sdk = sdk,
                };

                foreach (var groupElt in doc.Root.Elements("PropertyGroup"))
                {
                    foreach (var propElt in groupElt.Elements())
                    {
                        string name = propElt.Name.LocalName;

                        var prop = typeof(DotNetProject).GetProperty(name);

                        if (prop?.CanWrite == true)
                        {
                            prop.SetValue(project, propElt.Value);
                        }
                    }
                }

                return project;
            }
        }

        throw new FormatException("Invalid project format.");
    }

    public static bool TryParse(string path, [NotNullWhen(true)]out DotNetProject? project, [NotNullWhen(false)]out string? error)
    {
        try
        {
            project = Parse(path);
        }
        catch (Exception ex)
        {
            project = null;
            error = $"Load project file {path} failed: {ex.Message}";
            return false;
        }

        error = null;
        return true;
    }

    public static bool EvaluateTargetPath(string? projPath, string? configuration, [NotNullWhen(true)]out string? targetPath, [NotNullWhen(false)]out string? error)
    {
        DotNetProject? proj;

        if (projPath == null)
        {
            if (!TryFind(Environment.CurrentDirectory, out proj, out error))
            {
                targetPath = null;
                return false;
            }
        }
        else if (!TryParse(projPath, out proj, out error))
        {
            targetPath = null;
            return false;
        }

        targetPath = proj.GetTargetPath(configuration);
        return targetPath != null;
    }
}
