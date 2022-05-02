namespace Nabla.TypeScript;

public class AliasType : GenericType, INamedType
{
    public AliasType(string name, TypeBase target)
        : base(TS.FindParameters(target).CreateProxies())
    {
        Name = name;
        Target = Attach(target);
    }

    public string Name { get; }

    public TypeBase Target { get; }

    public override void Write(TypeWriter writer)
    {
        writer.Write("type").WriteSpace().Write(Name);

        writer.WriteGenericParameters(GenericParameters).WriteSpace();
        
        writer.Write('=').WriteSpace().WriteNode(Target).WriteSemicolon();
    }
}
