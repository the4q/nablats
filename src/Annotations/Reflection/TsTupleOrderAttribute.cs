namespace Nabla.TypeScript.Tool;

[AttributeUsage(AttributeTargetSets.PropertyOnly, AllowMultiple = false)]
public class TsTupleOrderAttribute : Attribute
{
    public TsTupleOrderAttribute(int order)
    {
        Order = order;
    }

    public int Order { get; }
}