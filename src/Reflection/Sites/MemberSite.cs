using System.Reflection;

namespace Nabla.TypeScript.Tool.Reflection;

internal class MemberSite : ISite, IPropertyMetaProvider<Type>, IPropertyRelatedMetaProvider<Type>
{
    public MemberSite(MemberInfo member, NullabilityInfo nullability, bool isTypeMember)
    {
        Member = member;
        Nullability = nullability;
        IsTypeMember = isTypeMember;
        UnderlyingType = Nullable.GetUnderlyingType(nullability.Type) ?? nullability.Type;
    }

    public MemberInfo Member { get; }

    public NullabilityInfo Nullability { get; }

    public bool IsTypeMember { get; }

    public Type MemberType => Nullability.Type;

    Type ISite.ReferencingType => MemberType;

    Type IPropertyMetaProvider<Type>.DeclaringSource => Member.DeclaringType!;

    public Type UnderlyingType { get; }

    public bool IsNullable => Nullability.ReadState != NullabilityState.NotNull || Nullability.WriteState != NullabilityState.NotNull;

    string IPropertyMetaProvider<Type>.Name => Member.Name;

    public bool IsReadOnly { get; init; }

    IPropertyMetaProvider<Type> IPropertyRelatedMetaProvider<Type>.Property => this;

    public GenericArgumentSite GenericArgument(int index)
    {
        if (index >= Nullability.GenericTypeArguments.Length)
            throw new ArgumentOutOfRangeException(nameof(index), "Invalid generic argument index.");

        return new(Member, Nullability.GenericTypeArguments[index], index);
    }

    public MemberSite ArrayElement()
    {
        if (Nullability.ElementType == null)
            throw new InvalidOperationException("Not a array.");

        return new(Member, Nullability.ElementType, false);
    }

    public static MemberSite Create(MemberInfo member)
    {
        NullabilityInfoContext context = new();
        NullabilityInfo info;
        
        if (member is PropertyInfo property)
            info = context.Create(property);
        else if (member is FieldInfo field)
            info = context.Create(field);
        else
            throw new ArgumentException("Invalid member type: " + member.MemberType);


        return new(member, info, true);
    }
}
