using Support.Numerics;

namespace Support.Geometrics.Hexagons.Pointy;

public enum E_TRANSLATION : byte
{
    UP_RIGHT,
    RIGHT,
    DOWN_RIGHT,
    DOWN_LEFT,
    LEFT,
    UP_LEFT
}

public struct Coordinate
{
    public Vec2<int> offset;
    public Coordinate(in Vec2<int> offset) => this.offset = offset;
    /// <summary>
    /// Modify the offset towards a direction by just one step.
    /// </summary>
    /// <param name="direction"></param>
    public void Translate(in E_TRANSLATION direction)
    {
        if (Toolbox.IsEven(offset.y))
        {   // Even cases
            offset += direction switch
            {
                E_TRANSLATION.UP_RIGHT => new Vec2<int>(0, -1),
                E_TRANSLATION.RIGHT => new Vec2<int>(1, 0),
                E_TRANSLATION.DOWN_RIGHT => new Vec2<int>(0, 1),
                E_TRANSLATION.DOWN_LEFT => new Vec2<int>(-1, 1),
                E_TRANSLATION.LEFT => new Vec2<int>(-1, 0),
                _ => new Vec2<int>(-1, -1)
            };
        }
        else
        {   // Odd cases
            offset += direction switch
            {
                E_TRANSLATION.UP_RIGHT => new Vec2<int>(1, -1),
                E_TRANSLATION.RIGHT => new Vec2<int>(1, 0),
                E_TRANSLATION.DOWN_RIGHT => new Vec2<int>(1, 1),
                E_TRANSLATION.DOWN_LEFT => new Vec2<int>(0, 1),
                E_TRANSLATION.LEFT => new Vec2<int>(-1, 0),
                _ => new Vec2<int>(0, -1)
            };
        }
    }
    /// <summary>
    /// Modify the offset towards a direction by certain amount of steps.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="steps"></param>
    public void Translate(in E_TRANSLATION direction, int steps)
    {
        for (int step = 0; step < steps; step++)
        {
            Translate(direction);
        }
    }
    public override readonly int GetHashCode() => offset.GetHashCode();
    public override readonly string ToString() => $"Coordinate({offset})";
}
