namespace Nabla.TypeScript.Tool;

[AttributeUsage(AttributeTargetSets.PropertyOnly, AllowMultiple = false)]
public class TsTypeOverrideAttribute : Attribute, ITypeOverrideInfo
{
    /// <summary>
    /// Initialize a new instance of <see cref="TsTypeOverrideAttribute"/> to indicate type of current property/field
    /// has a thrid-party or TypeScript utility type.
    /// </summary>
    /// <param name="typeName">Type name of thrid-party type.</param>
    /// <param name="typeParameters">Type parameters of third-party type. Supported parameter types are <see cref="string"/>, <see cref="TypeScriptPrimitive"/> and <see cref="System.Type"/>.</param>
    /// <remarks>
    /// <para>
    /// To use a third-party type, a <paramref name="moduleName"/> must be specified.
    /// This tool assumes that the third-party module is located at the same directory of output by default.
    /// There's no way to change this behavior in annotation. To specify a different location, you should use command-line argument.
    /// </para>
    /// <para>
    /// To use a TypeScript utility type, the <paramref name="moduleName"/> parameter should set to <c>null</c>.
    /// This tool won't validate the type name provided as a utility type, you must handle it yourself carefully.
    /// </para>
    /// </remarks>
    public TsTypeOverrideAttribute(string typeName, params object[] typeParameters)
    {
        TypeName = typeName;
        TypeParameters = typeParameters;
    }

    public string TypeName { get; }

    public object[]? TypeParameters { get; }

    /// <summary>
    /// Gets or sets the module name of where the type resided.
    /// </summary>
    /// <remarks>
    /// This property must be <c>null</c> if the type is primitive or utility type or internal handled type.
    /// </remarks>
    public string? ModuleName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int ArrayDepth { get; set; }

}