namespace Nabla.TypeScript.Tool;

public class PrimitiveCreator<TSouce> : TypeCreator<TSouce>
    where TSouce : notnull
{
    public PrimitiveCreator(TypeFactory<TSouce> factory) : base(factory)
    {
    }

    public override TypeBase CreateReference(TSouce source, IMetaProvider<TSouce> meta, object? state)
    {
        return TS.Native.Primitive(Descriptor.GetPrimitive(source, meta)!.Value);
    }

    public override HitTestResult HitTest(TSouce source)
    {
        return new(Descriptor.IsPrimitive(source));
    }
}
