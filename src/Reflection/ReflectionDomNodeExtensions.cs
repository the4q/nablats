using System.Reflection;

namespace Nabla.TypeScript.Tool.Reflection;

internal static class ReflectionDomNodeExtensions
{
    public static ISite GetSite(this DomNode type) => type.UserState as ISite
        ?? throw new InvalidOperationException("No site bound to this node.");

    public static ReferenceType WithSite(this ReferenceType type, ISite site)
    {
        return type;
    }

    //public static MemberInfo? GetMember(this DomNode? node)
    //{
    //    return node.GetMemberSite()?.Member;
    //}

    //public static MemberSite? GetMemberSite(this DomNode? node)
    //{
    //    while (node != null)
    //    {
    //        if (node.GetSite() is MemberSite site)
    //            return site;

    //        node = node.Parent;
    //    }

    //    return null;
    //}

    //public static MemberInfo? GetMember(this ISite site)
    //{
    //    if (site is MemberSite memberSite)
    //        return memberSite.Member;

    //    return null;
    //}

}
