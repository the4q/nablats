namespace Nabla.TypeScript.Tool;

public ref struct HitTestResult
{
    public HitTestResult(bool hit, object? state)
    {
        Hit = hit;
        State = state;
    }

    public HitTestResult(bool hit)
        : this(hit, null)
    {

    }

    public bool Hit { get; }

    public object? State { get; }

    public static implicit operator bool(HitTestResult result)
        => result.Hit;
}
