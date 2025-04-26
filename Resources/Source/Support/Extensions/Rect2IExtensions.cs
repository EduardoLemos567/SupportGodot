using System.Collections.Generic;
using Godot;

namespace Support.Extensions;

public static class Rect2IExtensions
{
    public static IEnumerable<Vector2I> EnumeratePositions(this Rect2I rect)
    {
        for (int x = 0; x < rect.Size.X; x++)
        {
            for (int y = 0; y < rect.Size.Y; y++)
            {
                yield return new Vector2I(rect.Position.X + x, rect.Position.Y + y);
            }
        }
    }
}
