using System;
using System.Collections.Generic;
using System.Numerics;
using Support.Diagnostics;
using Support.Numerics;

namespace Support;

public readonly struct Direction : IEquatable<Direction>
{
    public enum ENUM : byte
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
    private enum BITS : byte
    {
        UP = 0b0001,
        RIGHT = 0b0010,
        DOWN = 0b0100,
        LEFT = 0b1000,
    }
    public const byte SIZE = 8;
    private const byte HALF_SIZE = SIZE / 2;
    private static readonly float HALF_CIRCLE = float.Sin(float.Pi * 45 / 180);
    public static readonly Direction None = new(ENUM.NONE);
    public static readonly Direction Up = new(ENUM.UP);
    public static readonly Direction UpRight = new(ENUM.UP_RIGHT);
    public static readonly Direction Right = new(ENUM.RIGHT);
    public static readonly Direction DownRight = new(ENUM.DOWN_RIGHT);
    public static readonly Direction Down = new(ENUM.DOWN);
    public static readonly Direction DownLeft = new(ENUM.DOWN_LEFT);
    public static readonly Direction Left = new(ENUM.LEFT);
    public static readonly Direction UpLeft = new(ENUM.UP_LEFT);
    public readonly ENUM asEnum;
    public readonly bool IsNone => asEnum == ENUM.NONE;
    public readonly bool IsDiagonal => !IsNone && Toolbox.IsEven((int)asEnum);
    /// <summary>
    /// Check if its the diagonal top-left, also its opposite: down-right
    /// </summary>
    public readonly bool IsTopLeft => asEnum is ENUM.UP_LEFT or ENUM.DOWN_RIGHT;
    public readonly bool IsCardinal => !IsNone && !Toolbox.IsEven((int)asEnum);
    public readonly bool IsHorizontal => asEnum is ENUM.LEFT or ENUM.RIGHT;
    public readonly int AsInt => (int)asEnum;
    public readonly int AsIndex
    {
        get
        {
            Debug.Assert(!IsNone, "'direction' Direction cant be None.");
            return (int)asEnum - 1;
        }
    }
    public readonly byte AsBits => asEnum switch
    {
        ENUM.UP => (byte)BITS.UP,
        ENUM.UP_RIGHT => (byte)(BITS.UP | BITS.RIGHT),
        ENUM.RIGHT => (byte)BITS.RIGHT,
        ENUM.DOWN_RIGHT => (byte)(BITS.DOWN | BITS.RIGHT),
        ENUM.DOWN => (byte)BITS.DOWN,
        ENUM.DOWN_LEFT => (byte)(BITS.DOWN | BITS.LEFT),
        ENUM.LEFT => (byte)BITS.LEFT,
        ENUM.UP_LEFT => (byte)(BITS.UP | BITS.LEFT),
        _ => 0,
    };
    public readonly Direction Opposite => this + 4;
    public readonly Direction OppositeHorizontal => asEnum switch
    {
        ENUM.UP => Up,
        ENUM.UP_RIGHT => UpLeft,
        ENUM.RIGHT => Left,
        ENUM.DOWN_RIGHT => DownLeft,
        ENUM.DOWN => Down,
        ENUM.DOWN_LEFT => DownRight,
        ENUM.LEFT => Right,
        ENUM.UP_LEFT => UpRight,
        _ => None,
    };
    public readonly Direction OppositeVertical => asEnum switch
    {
        ENUM.UP => Down,
        ENUM.UP_RIGHT => DownRight,
        ENUM.RIGHT => Right,
        ENUM.DOWN_RIGHT => UpRight,
        ENUM.DOWN => Up,
        ENUM.DOWN_LEFT => UpLeft,
        ENUM.LEFT => Left,
        ENUM.UP_LEFT => DownLeft,
        _ => None,
    };
    private Direction(ENUM asEnum) => this.asEnum = asEnum;
    public static Direction FromEnum(ENUM asEnum)
    {
        if (asEnum < ENUM.NONE || asEnum > ENUM.UP_LEFT)
        {
            throw new ArgumentOutOfRangeException(nameof(asEnum), $"Value must be in [{ENUM.NONE}..{ENUM.UP_LEFT}].");
        }
        return new Direction(asEnum);
    }
    public static Direction FromInt(int asInt)
    {
        if (asInt < 0 || asInt > SIZE)
        {
            throw new ArgumentOutOfRangeException(nameof(asInt), $"Value must be in [0..{SIZE}].");
        }
        return new Direction((ENUM)asInt);
    }
    public static Direction FromIndex(int asIndex)
    {
        if (asIndex < 0 || asIndex >= SIZE)
        {
            throw new ArgumentOutOfRangeException(nameof(asIndex), $"Value must be in [0..{SIZE - 1}]");
        }
        return new Direction((ENUM)(asIndex + 1)); // +1 because NONE is at index 0
    }
    public static Direction FromBits(byte bits)
    {
        // TODO: fix and check
        return bits switch
        {
            0 => None,
            var b when (b & ((byte)BITS.UP | (byte)BITS.RIGHT)) == ((byte)BITS.UP | (byte)BITS.RIGHT) => UpRight,
            var b when (b & ((byte)BITS.RIGHT | (byte)BITS.DOWN)) == ((byte)BITS.RIGHT | (byte)BITS.DOWN) => DownRight,
            var b when (b & ((byte)BITS.DOWN | (byte)BITS.LEFT)) == ((byte)BITS.DOWN | (byte)BITS.LEFT) => DownLeft,
            var b when (b & ((byte)BITS.LEFT | (byte)BITS.UP)) == ((byte)BITS.LEFT | (byte)BITS.UP) => UpLeft,
            var b when (b & (byte)BITS.UP) != 0 => Up,
            var b when (b & (byte)BITS.RIGHT) != 0 => Right,
            var b when (b & (byte)BITS.DOWN) != 0 => Down,
            var b when (b & (byte)BITS.LEFT) != 0 => Left,
            _ => None,
        };
    }
    public static Direction operator +(in Direction direction, int value) => FromInt(Toolbox.PositiveModulo(direction.AsInt + value, SIZE));
    public static Direction operator -(in Direction direction, int value) => FromInt(Toolbox.PositiveModulo(direction.AsInt - value, SIZE));
    public readonly Vec2<int> ToVec2Int()
    {
        Vec2<int> result = default;
        result.x = asEnum switch
        {
            ENUM.RIGHT or ENUM.DOWN_RIGHT or ENUM.UP_RIGHT => 1,
            ENUM.LEFT or ENUM.DOWN_LEFT or ENUM.UP_LEFT => -1,
            _ => 0,
        };
        result.y = asEnum switch
        {
            ENUM.UP or ENUM.UP_RIGHT or ENUM.UP_LEFT => -1,
            ENUM.DOWN or ENUM.DOWN_RIGHT or ENUM.DOWN_LEFT => 1,
            _ => 0,
        };
        return result;
    }
    public readonly Vec2<float> ToVec2Float() => ToVec2Int().CastTo<float>() * (IsDiagonal ? HALF_CIRCLE : 1);
    /// <summary>
    /// Example: if we are UP_RIGHT, it will return true for: UP, RIGHT, UP_RIGHT.
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public readonly bool IsActive(in Direction direction)
    {
        if (direction.IsCardinal)
        {
            var bits0 = AsBits;
            var bits1 = direction.AsBits;
            return (bits0 & bits1) != 0;
        }
        else
        {
            return this == direction;
        }

    }
    public override readonly string ToString()
    {
        return $@"Direction({asEnum switch
        {
            ENUM.UP => "Up",
            ENUM.UP_RIGHT => "UpRight",
            ENUM.RIGHT => "Right",
            ENUM.DOWN_RIGHT => "DownRight",
            ENUM.DOWN => "Down",
            ENUM.DOWN_LEFT => "DownLeft",
            ENUM.LEFT => "Left",
            ENUM.UP_LEFT => "UpLeft",
            _ => "None"
        }})";
    }
    public override readonly bool Equals(object? obj) => obj is Direction direction && Equals(direction);
    public readonly bool Equals(Direction other) => this == other;
    public override readonly int GetHashCode() => HashCode.Combine(asEnum);
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
        var result = (int)other.asEnum - (int)asEnum;
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
        var result = (int)asEnum - (int)other.asEnum;
        return result is 4 or (-4);
    }
    public static bool operator ==(in Direction one, in Direction other) => one.asEnum == other.asEnum;
    public static bool operator !=(in Direction one, in Direction other) => one.asEnum != other.asEnum;
    public static Vec2<int> operator +(in Vec2<int> left, in Direction right) => left + right.ToVec2Int();
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
