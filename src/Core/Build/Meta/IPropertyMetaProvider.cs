namespace Nabla.TypeScript.Tool;

public interface IPropertyMetaProvider<TSource> : IMetaProvider<TSource>
    where TSource : notnull
{
    string Name { get; }

    //ITypeOverrideInfo? TypeOverride { get; }

    TSource DeclaringSource { get; }

    bool IsReadOnly { get; }

}

//public abstract class TypeMemberInfo<TSource> : IMetaProvider<TSource>, ITypeMember
//    where TSource : notnull
//{
//    public TypeMemberInfo(string name, TSource source, object clrMember)
//    {
//        Name = name;
//        Source = source;
//        ClrMember = clrMember;
//    }

//    public bool IsNullable { get; set; }

//    public abstract bool IsTypeMember { get; }

//    public string Name { get; }

//    public bool IsReadOnly { get; set; }

//    public TSource Source { get; }

//    public object ClrMember { get; }

//}

//public class TypePropertyInfo<TSource> : TypeMemberInfo<TSource>
//    where TSource : notnull
//{
//    public TypePropertyInfo(string name, TSource source, object clrMember)
//        : base(name, source, clrMember)
//    {
//    }

//    public override bool IsTypeMember => true;

//    public TypeOverrideInfo? TypeOverride { get; set; }

//}

//public class GenericArgumentMetaProvider<TSource> : IMetaProvider<TSource>, ITypeMember
//    where TSource : notnull
//{
//    public GenericArgumentMetaProvider(TypeMemberInfo<TSource> member, int argumentIndex)
//    {
//        Member = member;
//        ArgumentIndex = argumentIndex;
//    }

//    public bool IsNullable { get; private set; }

//    public bool IsTypeMember => false;

//    public TSource Source => throw new NotImplementedException();

//    public TypeMemberInfo<TSource> Member { get; }

//    public int ArgumentIndex { get; }

//    public string Name => ((ITypeMember)Member).Name;

//    public object ClrMember => ((ITypeMember)Member).ClrMember;

//    public static GenericArgumentMetaProvider<TSource> Create(TypeMemberInfo<TSource> member, int argIndex)
//    {

//    }
//}
