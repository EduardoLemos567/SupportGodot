using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Support.Extensions;

public static class Rect2IExtensions
{
    public static IEnumerable<Vector2I> EnumeratePositions(this Rect2I rect)
    {
        return from p in rect.Size.EnumeratePositions() select p + rect.Position;
    }
}
