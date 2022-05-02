namespace Nabla.TypeScript.Tool;

public class RecordTypeInfo<TSource>
    where TSource: notnull
{
    public RecordTypeInfo(TSource key, TSource value)
    {
        Key = key;
        Value = value;
    }

    public TSource Key { get; }

    public TSource Value { get; }
}