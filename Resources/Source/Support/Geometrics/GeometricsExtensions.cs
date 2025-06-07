using System.Collections.Generic;
using System.Linq;
using Support.Numerics;

namespace Support.Geometrics;

public static class GeometricsExtensions
{
    public static IEnumerable<Vec2<int>> EnumeratePositions(this Rectangle<int> rect)
    {
        return from p in rect.Size.EnumeratePositions() select p + rect.min;
    }
}
