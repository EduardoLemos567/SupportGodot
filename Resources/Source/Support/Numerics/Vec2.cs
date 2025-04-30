using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Support.Numerics;

[StructLayout(LayoutKind.Sequential)]
public struct Vec2<N> : IVectorNumber<N> where N : INumber<N>
{
    public static readonly Vec2<N> Zero = CreateFrom(0, 0);
    public static readonly Vec2<N> One = CreateFrom(1, 1);
    public static readonly Vec2<N> Half = CreateFrom(0.5f, 0.5f);
    public static readonly Vec2<N> Up = CreateFrom(0, 1);
    public static readonly Vec2<N> Down = CreateFrom(0, -1);
    public static readonly Vec2<N> Left = CreateFrom(1, 0);
    public static readonly Vec2<N> Right = CreateFrom(-1, 0);
    public N x;
    public N y;
    public readonly int DIMENSIONS => 2;
    #region SWIZZLE
    public N this[int x]
    {
        readonly get => x switch
        {
            0 => this.x,
            1 => y,
            _ => throw new ArgumentException("Index out of valid range")
        };
        set
        {
            switch (x)
            {
                case 0: this.x = value; return;
                case 1: y = value; return;
                default: throw new ArgumentException("Index out of valid range");
            }
        }
    }
    public Vec2<N> this[int x, int y]
    {
        readonly get => new(this[x], this[y]);
        set
        {
            this[x] = value.x;
            this[y] = value.y;
        }
    }
    #endregion SWIZZLE
    #region CONSTRUCTORS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec2(N x = default!, N y = default!)
    {
        this.x = x;
        this.y = y;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec2(N all) { x = all; y = all; }
    #endregion CONSTRUCTORS
    #region FUNCTIONS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly N SqrMagnitude() => x * x + y * y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec2<N> Abs() => new(N.Abs(x), N.Abs(y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly N MinValue() => N.Min(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly N MaxValue() => N.Max(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly N SqrDistance(in Vec2<N> target) => (target - this).SqrMagnitude();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec2<N> Min(in Vec2<N> target) => new(
        N.Min(x, target.x),
        N.Min(y, target.y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec2<N> Max(in Vec2<N> target) => new(
        N.Max(x, target.x),
        N.Max(y, target.y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec2<N> Clamp(in Vec2<N> min, in Vec2<N> max) => new(
        N.Clamp(x, min.x, max.x),
        N.Clamp(y, min.y, max.y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object? target) => target is Vec2<N> Vec2 && Equals(Vec2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(in Vec2<N> target) => (this == target).AllTrue;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly string ToString() => $"Vec2<{typeof(N).Name}>({x}, {y})";
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode() => HashCode.Combine(x, y);
    #endregion FUNCTIONS
    #region MATH_OPERATORS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> operator -(in Vec2<N> v) => new(-v.x, -v.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> operator +(in Vec2<N> v1, in Vec2<N> v2) => new(
        v1.x + v2.x,
        v1.y + v2.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> operator -(in Vec2<N> v1, in Vec2<N> v2) => new(
        v1.x - v2.x,
        v1.y - v2.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> operator *(in Vec2<N> v1, in Vec2<N> v2) => new(
        v1.x * v2.x,
        v1.y * v2.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> operator /(in Vec2<N> v1, in Vec2<N> v2) => new(
        v1.x / v2.x,
        v1.y / v2.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> operator %(in Vec2<N> v1, in Vec2<N> v2) => new(
        v1.x % v2.x,
        v1.y % v2.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> operator +(in Vec2<N> v, in N n) => new(
        v.x + n,
        v.y + n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> operator -(in Vec2<N> v, in N n) => new(
        v.x - n,
        v.y - n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> operator *(in Vec2<N> v, in N n) => new(
        v.x * n,
        v.y * n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> operator /(in Vec2<N> v, in N n) => new(
        v.x / n,
        v.y / n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> operator %(in Vec2<N> v, in N n) => new(
        v.x % n,
        v.y % n);
    #endregion MATH_OPERATORS
    #region LOGIC_OPERATORS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool2 operator ==(in Vec2<N> a, in Vec2<N> b) => new(
        a.x == b.x,
        a.y == b.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool2 operator !=(in Vec2<N> a, in Vec2<N> b) => new(
        a.x != b.x,
        a.y != b.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool2 operator >(in Vec2<N> v1, in Vec2<N> v2) => new(
        v1.x > v2.x,
        v1.y > v2.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool2 operator <(in Vec2<N> v1, in Vec2<N> v2) => new(
        v1.x < v2.x,
        v1.y < v2.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool2 operator >=(in Vec2<N> v1, in Vec2<N> v2) => new(
        v1.x >= v2.x,
        v1.y >= v2.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool2 operator <=(in Vec2<N> v1, in Vec2<N> v2) => new(
        v1.x <= v2.x,
        v1.y <= v2.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool2 operator >(in Vec2<N> v, in N n) => new(
        v.x > n,
        v.y > n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool2 operator <(in Vec2<N> v, in N n) => new(
        v.x < n,
        v.y < n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool2 operator >=(in Vec2<N> v, in N n) => new(
        v.x >= n,
        v.y >= n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool2 operator <=(in Vec2<N> v, in N n) => new(
        v.x <= n,
        v.y <= n);
    #endregion LOGIC_OPERATORS
    #region CONVERTERS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec2<TOutput> CastTo<TOutput>() where TOutput : INumber<TOutput> => Vec2<TOutput>.CreateFrom(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> CreateFrom<TInput>(TInput x, TInput y)
        where TInput : INumber<TInput> => new(
        N.CreateChecked(x),
        N.CreateChecked(y));
    #endregion CONVERTERS
}