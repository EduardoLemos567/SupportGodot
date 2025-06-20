using System;
using System.Collections.Generic;
using System.Numerics;
using Support.Diagnostics;
using Support.Numerics;

namespace Support;

public readonly struct Direction : IEquatable<Direction>
{
    public enum DIRECTIONS : byte
    {
        NONE,
        UP,
        UP_RIGHT,
        RIGHT,
        DOWN_RIGHT,
        DOWN,
        DOWN_LEFT,
        LEFT,
        UP_LEFT,
    }
    private const byte MIN = 1;
    public const byte SIZE = 8;
    private const byte HALF_SIZE = SIZE / 2;
    private static readonly float HALF_CIRCLE = float.Sin(float.Pi * 45 / 180);
    public static readonly Direction None = new(DIRECTIONS.NONE);
    public static readonly Direction Up = new(DIRECTIONS.UP);
    public static readonly Direction UpRight = new(DIRECTIONS.UP_RIGHT);
    public static readonly Direction Right = new(DIRECTIONS.RIGHT);
    public static readonly Direction DownRight = new(DIRECTIONS.DOWN_RIGHT);
    public static readonly Direction Down = new(DIRECTIONS.DOWN);
    public static readonly Direction DownLeft = new(DIRECTIONS.DOWN_LEFT);
    public static readonly Direction Left = new(DIRECTIONS.LEFT);
    public static readonly Direction UpLeft = new(DIRECTIONS.UP_LEFT);
    public readonly DIRECTIONS AsEnum;
    public readonly bool IsNone => AsEnum == DIRECTIONS.NONE;
    public readonly bool IsDiagonal => !IsNone && Toolbox.IsEven((int)AsEnum);
    /// <summary>
    /// Check if its the diagonal top-left, also its opposite: down-right
    /// </summary>
    public readonly bool IsTopLeft => AsEnum is DIRECTIONS.UP_LEFT or DIRECTIONS.DOWN_RIGHT;
    public readonly bool IsCardinal => !IsNone && !Toolbox.IsEven((int)AsEnum);
    public readonly bool IsHorizontal => AsEnum is DIRECTIONS.LEFT or DIRECTIONS.RIGHT;
    public readonly int AsIndex
    {
        get
        {
            Debug.Assert(!IsNone, "'direction' Direction cant be None.");
            return (int)AsEnum - 1;
        }
    }
    public readonly Direction Opposite => this + 4;
    public readonly Direction OppositeHorizontal => AsEnum switch
    {
        DIRECTIONS.UP => Up,
        DIRECTIONS.UP_RIGHT => UpLeft,
        DIRECTIONS.RIGHT => Left,
        DIRECTIONS.DOWN_RIGHT => DownLeft,
        DIRECTIONS.DOWN => Down,
        DIRECTIONS.DOWN_LEFT => DownRight,
        DIRECTIONS.LEFT => Right,
        DIRECTIONS.UP_LEFT => UpRight,
        _ => None,
    };
    public readonly Direction OppositeVertical => AsEnum switch
    {
        DIRECTIONS.UP => Down,
        DIRECTIONS.UP_RIGHT => DownRight,
        DIRECTIONS.RIGHT => Right,
        DIRECTIONS.DOWN_RIGHT => UpRight,
        DIRECTIONS.DOWN => Up,
        DIRECTIONS.DOWN_LEFT => UpLeft,
        DIRECTIONS.LEFT => Left,
        DIRECTIONS.UP_LEFT => DownLeft,
        _ => None,
    };
    public Direction(DIRECTIONS asEnum) => AsEnum = asEnum;
    public static Direction operator +(in Direction direction, int value)
    {
        var result = ((direction.IsNone ? 0 : ((int)direction.AsEnum - MIN)) + value) % SIZE;
        return new((DIRECTIONS)(result + (result >= 0 ? 0 : SIZE) + MIN));
    }
    public static Direction operator -(in Direction direction, int value)
    {
        var result = ((direction.IsNone ? 0 : ((int)direction.AsEnum - MIN)) - value) % SIZE;
        return new((DIRECTIONS)(result + (result >= 0 ? 0 : SIZE) + MIN));
    }
    public readonly Vec2<int> ToInt()
    {
        return AsEnum switch
        {
            DIRECTIONS.UP => new(0, -1),
            DIRECTIONS.UP_RIGHT => new(1, -1),
            DIRECTIONS.RIGHT => new(1, 0),
            DIRECTIONS.DOWN_RIGHT => new(1, 1),
            DIRECTIONS.DOWN => new(0, 1),
            DIRECTIONS.DOWN_LEFT => new(-1, 1),
            DIRECTIONS.LEFT => new(-1, 0),
            DIRECTIONS.UP_LEFT => new(-1, -1),
            _ => Vec2<int>.Zero,
        };
    }
    public readonly Vec2<float> ToFloat() => ToInt().CastTo<float>() * (IsDiagonal ? HALF_CIRCLE : 1);
    public override readonly string ToString()
    {
        return $@"Direction({AsEnum switch
        {
            DIRECTIONS.UP => "Up",
            DIRECTIONS.UP_RIGHT => "UpRight",
            DIRECTIONS.RIGHT => "Right",
            DIRECTIONS.DOWN_RIGHT => "DownRight",
            DIRECTIONS.DOWN => "Down",
            DIRECTIONS.DOWN_LEFT => "DownLeft",
            DIRECTIONS.LEFT => "Left",
            DIRECTIONS.UP_LEFT => "UpLeft",
            _ => "None"
        }})";
    }
    public override readonly bool Equals(object? obj) => obj is Direction direction && Equals(direction);
    public readonly bool Equals(Direction other) => this == other;
    public override readonly int GetHashCode() => HashCode.Combine(AsEnum);
    /// <summary>
    /// The radial distance of the directions.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public readonly int AbsoluteDistance(in Direction other)
    {
        var result = ClockwiseDistance(other);
        return result > HALF_SIZE ? -(result - SIZE) : result;
    }
    public readonly int RelativeDistance(in Direction other)
    {
        return ClockwiseDistance(other) switch
        {
            int cw when cw < 4 => cw,
            int cw when cw > 4 => cw - SIZE,
            _ => 0,
        };
    }
    public readonly int ClockwiseDistance(in Direction other)
    {
        Debug.Assert(!IsNone, "'this' Direction cant be None.");
        Debug.Assert(!other.IsNone, "'other' Direction cant be None.");
        var result = (int)other.AsEnum - (int)AsEnum;
        return result < 0 ? result + SIZE : result;
    }
    public readonly int CounterClockwiseDistance(in Direction other) => other.ClockwiseDistance(this);
    /// <summary>
    /// Check of their angles form 180 degree. 
    /// If one direction is the opposite of this.
    /// </summary>
    /// <param name="other">Other direction to be compared.</param>
    /// <returns>True if is the opposite</returns>
    public readonly bool IsOpposite(in Direction other)
    {
        Debug.Assert(!IsNone, "'this' Direction cant be None.");
        Debug.Assert(!other.IsNone, "'other' Direction cant be None.");
        var result = (int)AsEnum - (int)other.AsEnum;
        return result is 4 or (-4);
    }
    public static bool operator ==(in Direction one, in Direction other) => one.AsEnum == other.AsEnum;
    public static bool operator !=(in Direction one, in Direction other) => one.AsEnum != other.AsEnum;
    public static Vec2<int> operator +(in Vec2<int> left, in Direction right) => left + right.ToInt();
    public static Direction FromDelta<N>(in Vec2<N> delta) where N : INumber<N>
    {
        return (delta.x, delta.y) switch
        {
            ( > 0, 0) => Right,
            ( < 0, 0) => Left,
            (0, < 0) => Up,
            (0, > 0) => Down,
            ( > 0, < 0) => UpRight,
            ( < 0, > 0) => DownLeft,
            ( > 0, > 0) => DownRight,
            ( < 0, < 0) => UpLeft,
            _ => None,
        };
    }
    public static IEnumerable<Direction> LoopClockwise(Direction from = default, Direction to = default, int stride = 1)
    {
        if (from == default) { from = Up; }
        if (to == default) { to = from; }
        do
        {
            yield return from;
            from += stride;
        } while (from != to);
        yield return to;
    }
    public static IEnumerable<Direction> LoopCounterClockwise(Direction from = default, Direction to = default, int stride = 1)
    {
        if (from == default) { from = Up; }
        if (to == default) { to = from; }
        do
        {
            yield return from;
            from -= stride;
        } while (from != to);
        yield return to;
    }
    public static IEnumerable<Direction> LoopClockwiseCardinalOnly()
    {
        return LoopClockwise(Up, Left, 2);
    }
    public static IEnumerable<Direction> LoopClockwiseDiagonalOnly()
    {
        return LoopClockwise(UpRight, UpLeft, 2);
    }
}
