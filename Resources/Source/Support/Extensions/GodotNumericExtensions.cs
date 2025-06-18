using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

namespace Support.Extensions;

public static class GodotNumericExtensions
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 ToXZ(this Vector2 vec, float y = 0)
    {
        return new Vector3(vec.X, y, vec.Y);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 TakeXZ(this Vector3 vec)
    {
        return new Vector2(vec.X, vec.Z);
    }
}
