namespace Nabla.TypeScript.Tool;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class TsTypeDiscovererAttribute : Attribute
{
    public TsTypeDiscovererAttribute(Type discovererType)
    {
        DiscovererType = discovererType;
    }

    public Type DiscovererType { get; }
}