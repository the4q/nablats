namespace Nabla.TypeScript.Tool;

public interface ITypeSourceDiscoverer
{
    IEnumerable<object> Discover();
}