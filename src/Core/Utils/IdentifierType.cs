namespace Nabla.TypeScript.Tool
{
    internal sealed class IdentifierType : TypeBase
    {
        public IdentifierType(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override void Write(TypeWriter writer)
        {
            writer.Write(Name);
        }
    }
}
