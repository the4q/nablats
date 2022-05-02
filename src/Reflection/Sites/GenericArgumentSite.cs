using System.Reflection;

namespace Nabla.TypeScript.Tool.Reflection;

internal class GenericArgumentSite : MemberSite
{
    public GenericArgumentSite(MemberInfo member, NullabilityInfo nullability, int argumentIndex)
        : base(member, nullability, false)
    {
        ArgumentIndex = argumentIndex;
    }

    public int ArgumentIndex { get; }
}
