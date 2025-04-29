using System.Numerics;

namespace Support.Numerics;

public interface IVectorNumber<N> : IVector where N : INumber<N>
{
    public static readonly N PROXIMITY_DISTANCE = N.CreateChecked(Toolbox.PROXIMITY_DISTANCE);
    N this[int index] { get; set; }
}
