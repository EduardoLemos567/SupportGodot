using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Support.Numerics;

[StructLayout(LayoutKind.Sequential)]
public struct Vec3<N> : IVectorNumber<N> where N : INumber<N>
{
    public static readonly Vec3<N> Zero = CreateFrom(0, 0, 0);
    public static readonly Vec3<N> One = CreateFrom(1, 1, 1);
    public static readonly Vec3<N> Half = CreateFrom(0.5f, 0.5f, 0.5f);
    public static readonly Vec3<N> Up = CreateFrom(0, 1, 0);
    public static readonly Vec3<N> Down = CreateFrom(0, -1, 0);
    public static readonly Vec3<N> Left = CreateFrom(1, 0, 0);
    public static readonly Vec3<N> Right = CreateFrom(-1, 0, 0);
    public static readonly Vec3<N> Front = CreateFrom(0, 0, 1);
    public static readonly Vec3<N> Back = CreateFrom(0, 0, -1);
    public N x;
    public N y;
    public N z;
    public readonly int DIMENSIONS => 3;
    #region SWIZZLE
    public N this[int x]
    {
        readonly get => x switch
        {
            0 => this.x,
            1 => y,
            2 => z,
            _ => throw new ArgumentException("Index out of valid range")
        };
        set
        {
            switch (x)
            {
                case 0: this.x = value; return;
                case 1: y = value; return;
                case 2: z = value; return;
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
    public Vec3<N> this[int x, int y, int z]
    {
        readonly get => new(this[x], this[y], this[z]);
        set
        {
            this[x] = value.x;
            this[y] = value.y;
            this[z] = value.z;
        }
    }
    #endregion SWIZZLE
    #region CONSTRUCTORS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec3(N x = default!, N y = default!, N z = default!)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec3(N all) { x = all; y = all; z = all; }
    #endregion CONSTRUCTORS
    #region FUNCTIONS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly N SqrMagnitude() => x * x + y * y + z * z;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec3<N> Abs() => new(N.Abs(x), N.Abs(y), N.Abs(z));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly N MinValue() => N.Min(x, N.Min(y, z));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly N MaxValue() => N.Max(x, N.Max(y, z));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly N SqrDistance(in Vec3<N> other) => (other - this).SqrMagnitude();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec3<N> Min(in Vec3<N> other) => new(
        N.Min(x, other.x),
        N.Min(y, other.y),
        N.Min(z, other.z));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec3<N> Max(in Vec3<N> other) => new(
        N.Max(x, other.x),
        N.Max(y, other.y),
        N.Max(z, other.z));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec3<N> Clamp(in Vec3<N> min, in Vec3<N> max) => new(
        N.Clamp(x, min.x, max.x),
        N.Clamp(y, min.y, max.y),
        N.Clamp(z, min.z, max.z));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly N Multi() => x * y * z;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object? obj) => obj is Vec3<N> Vec3 && Equals(Vec3);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(in Vec3<N> other) => (this == other).AllTrue;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly string ToString() => $"({x}, {y}, {z})";
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode() => HashCode.Combine(x, y, z);
    #endregion FUNCTIONS
    #region MATH_OPERATORS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> operator -(in Vec3<N> v) => new(-v.x, -v.y, -v.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> operator +(in Vec3<N> v1, in Vec3<N> v2) => new(
        v1.x + v2.x,
        v1.y + v2.y,
        v1.z + v2.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> operator -(in Vec3<N> v1, in Vec3<N> v2) => new(
        v1.x - v2.x,
        v1.y - v2.y,
        v1.z - v2.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> operator *(in Vec3<N> v1, in Vec3<N> v2) => new(
        v1.x * v2.x,
        v1.y * v2.y,
        v1.z * v2.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> operator /(in Vec3<N> v1, in Vec3<N> v2) => new(
        v1.x / v2.x,
        v1.y / v2.y,
        v1.z / v2.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> operator %(in Vec3<N> v1, in Vec3<N> v2) => new(
        v1.x % v2.x,
        v1.y % v2.y,
        v1.z % v2.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> operator +(in Vec3<N> v, in N n) => new(
        v.x + n,
        v.y + n,
        v.z + n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> operator -(in Vec3<N> v, in N n) => new(
        v.x - n,
        v.y - n,
        v.z - n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> operator *(in Vec3<N> v, in N n) => new(
        v.x * n,
        v.y * n,
        v.z * n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> operator /(in Vec3<N> v, in N n) => new(
        v.x / n,
        v.y / n,
        v.z / n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> operator %(in Vec3<N> v, in N n) => new(
        v.x % n,
        v.y % n,
        v.z % n);
    #endregion MATH_OPERATORS
    #region LOGIC_OPERATORS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool3 operator ==(in Vec3<N> a, Vec3<N> b) => new(
        a.x == b.x,
        a.y == b.y,
        a.z == b.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool3 operator !=(in Vec3<N> a, Vec3<N> b) => new(
        a.x != b.x,
        a.y != b.y,
        a.z != b.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool3 operator >(in Vec3<N> v1, in Vec3<N> v2) => new(
        v1.x > v2.x,
        v1.y > v2.y,
        v1.z > v2.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool3 operator <(in Vec3<N> v1, in Vec3<N> v2) => new(
        v1.x < v2.x,
        v1.y < v2.y,
        v1.z < v2.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool3 operator >=(in Vec3<N> v1, in Vec3<N> v2) => new(
        v1.x >= v2.x,
        v1.y >= v2.y,
        v1.z >= v2.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool3 operator <=(in Vec3<N> v1, in Vec3<N> v2) => new(
        v1.x <= v2.x,
        v1.y <= v2.y,
        v1.z <= v2.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool3 operator >(in Vec3<N> v, in N n) => new(
        v.x > n,
        v.y > n,
        v.z > n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool3 operator <(in Vec3<N> v, in N n) => new(
        v.x < n,
        v.y < n,
        v.z < n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool3 operator >=(in Vec3<N> v, in N n) => new(
        v.x >= n,
        v.y >= n,
        v.z >= n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool3 operator <=(in Vec3<N> v, in N n) => new(
        v.x <= n,
        v.y <= n,
        v.z <= n);
    #endregion LOGIC_OPERATORS
    #region CONVERTERS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec3<TOutput> CastTo<TOutput>() where TOutput : INumber<TOutput> => Vec3<TOutput>.CreateFrom(x, y, z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> CreateFrom<TInput>(TInput x, TInput y, TInput z)
        where TInput : INumber<TInput> => new(
        N.CreateChecked(x),
        N.CreateChecked(y),
        N.CreateChecked(z));
    #endregion CONVERTERS
}
