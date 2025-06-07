using System.Collections.Generic;
using Godot;

namespace Support.Extensions;

public static class Vector2IExtensions
{
    /// <summary>
    /// Loop through X first than next Y.
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static IEnumerable<Vector2I> EnumeratePositions(this Vector2I vec)
    {
        for (int y = 0; y < vec.Y; y++)
        {
            for (int x = 0; x < vec.X; x++)
            {
                yield return new Vector2I(x, y);
            }
        }
    }
}
