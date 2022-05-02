namespace Nabla.TypeScript;

public class ArrayType : TypeBase
{
    public ArrayType(TypeBase elementType)
    {
        ElementType = Attach(elementType);
    }

    public TypeBase ElementType { get; }

    public override void Write(TypeWriter writer)
    {
        writer.WriteNode(ElementType).Write("[]");
    }

}
