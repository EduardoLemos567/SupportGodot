using System.Numerics;

namespace Support.Numerics;

public readonly struct Range<N> where N : INumber<N>
{
    public N Min { get; }
    public N Max { get; }
    public readonly N Mid => Delta / N.CreateTruncating(2);
    public readonly N Delta => Max - Min;
    public Range(N min, N max)
    {
        if (min > max)
        {
            (min, max) = (max, min);
        }
        Min = min;
        Max = max;
    }
    public readonly N PositiveModulo(in N value) => Toolbox.PositiveModulo(value, Delta) + Min;
    public readonly N Clamp(in N value) => N.Clamp(value, Min, Max);
    public readonly int CompareTo(in N value)
    {
        if (value < Min) { return -1; }
        if (value > Max) { return 1; }
        return 0;
    }
}
