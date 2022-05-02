namespace Nabla.TypeScript;

[Serializable]
public class CodeException : Exception
{
    public CodeException() { }
    public CodeException(string message) : base(message) { }
    public CodeException(string message, Exception inner) : base(message, inner) { }
    protected CodeException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}