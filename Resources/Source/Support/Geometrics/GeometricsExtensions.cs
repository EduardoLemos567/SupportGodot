using System.Collections.Generic;
using Support.Numerics;

namespace Support.Geometrics;

public static class GeometricsExtensions
{
    public static IEnumerable<Vec2<int>> EnumeratePositions(this Rectangle<int> rect)
    {
        for (int x = 0; x < rect.Size.x; x++)
        {
            for (int y = 0; y < rect.Size.y; y++)
            {
                yield return new(rect.min.x + x, rect.min.y + y);
            }
        }
    }
}
