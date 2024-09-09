using System.Numerics;

namespace Support.Numerics;

public interface IVectorNumber<N> : IVector where N : INumber<N>
{
    public static readonly N PROXIMITY_DISTANCE = N.CreateChecked(0.001);
    N this[int index] { get; }
}
