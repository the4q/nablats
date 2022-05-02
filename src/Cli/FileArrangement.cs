namespace Nabla.TypeScript.Tool;

/// <summary>
/// Represents a enum which indicates how TypeScript files should be arranged.
/// </summary>
/// <remarks>
/// Developers can specify which file should a type be placed.
/// For those unspecified types, this enum defines how they should be arranged.
/// </remarks>
public enum FileArrangement
{
    /// <summary>
    /// Types of module name unspecified will be placed in the file of type which first reference certain type.
    /// </summary>
    Explicit,
    /// <summary>
    /// Types of module name unspecified will be placed in modules corresponding to it declaring assembly.
    /// </summary>
    Nature,
    /// <summary>
    /// All types will be placed in a single module reguardless of develper specification.
    /// </summary>
    Single,
    /// <summary>
    /// Types of module name unspecified will be placed in file with name correspond to its CLR type namespace.
    /// </summary>
    Namespace,
}
