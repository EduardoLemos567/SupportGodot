using System.Numerics;

namespace Support.Numerics;

public struct Range<T> : IConstraintable where T : INumber<T>
{
    private T min;
    private T max;
    public T Min
    {
        readonly get => min;
        set { min = value; EnforceConstraint(); }
    }
    public T Max
    {
        readonly get => max;
        set { max = value; EnforceConstraint(); }
    }
    public readonly T Mid => Delta / T.CreateTruncating(2);
    public readonly T Delta => max - min;
    public Range(in T min, in T max)
    {
        this.min = min;
        this.max = max;
        EnforceConstraint();
    }
    public void EnforceConstraint() { if (min > max) { (min, max) = (max, min); } }
    public readonly T Modulo(in T value) => ((value - Min) % Delta) + Min;
    public readonly T Clamp(in T value) => T.Clamp(value, min, max);
}
