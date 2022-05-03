namespace Nabla.TypeScript;

public class FileReference : Reference
{
    public FileReference(TypeFile target)
        : base(target)
    {

    }

    public new TypeFile Target => (TypeFile)base.Target;

    public override DomNodeKind Kind => DomNodeKind.NamespaceImport;

    public override void Write(TypeWriter writer)
    {
        writer.Write("/// <reference ")
            .WriteAttribute("path", Target.GetPath(DeclaringFile!, true))
            .Write(" />");
    }
}