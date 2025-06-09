using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Support.Numerics;

[StructLayout(LayoutKind.Sequential)]
public struct Vec4<N> : IVectorNumber<N> where N : INumber<N>
{
    public static readonly Vec4<N> Zero = CreateFrom(0, 0, 0, 0);
    public static readonly Vec4<N> One = CreateFrom(1, 1, 1, 1);
    public static readonly Vec4<N> Half = CreateFrom(0.5f, 0.5f, 0.5f, 0.5f);
    public N x;
    public N y;
    public N z;
    public N w;
    public readonly int DIMENSIONS => 4;
    #region SWIZZLE
    public N this[int x]
    {
        readonly get => x switch
        {
            0 => this.x,
            1 => y,
            2 => z,
            3 => w,
            _ => throw new ArgumentException("Index out of valid range")
        };
        set
        {
            switch (x)
            {
                case 0: this.x = value; return;
                case 1: y = value; return;
                case 2: z = value; return;
                case 3: w = value; return;
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
    public Vec4<N> this[int x, int y, int z, int w]
    {
        readonly get => new(this[x], this[y], this[z], this[w]);
        set
        {
            this[x] = value.x;
            this[y] = value.y;
            this[z] = value.z;
            this[w] = value.w;
        }
    }
    #endregion SWIZZLE
    #region CONSTRUCTORS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec4(N x = default!, N y = default!, N z = default!, N w = default!)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec4(N all) { x = all; y = all; z = all; w = all; }
    #endregion CONSTRUCTORS
    #region FUNCTIONS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly N SqrMagnitude() => x * x + y * y + z * z;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec4<N> Abs() => new(N.Abs(x), N.Abs(y), N.Abs(z), N.Abs(w));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly N MinValue() => N.Min(x, N.Min(y, N.Min(z, w)));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly N MaxValue() => N.Max(x, N.Max(y, N.Max(z, w)));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly N SqrDistance(in Vec4<N> other) => (other - this).SqrMagnitude();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec4<N> Min(in Vec4<N> other) => new(
        N.Min(x, other.x),
        N.Min(y, other.y),
        N.Min(z, other.z),
        N.Min(w, other.w));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec4<N> Max(in Vec4<N> other) => new(
        N.Max(x, other.x),
        N.Max(y, other.y),
        N.Max(z, other.z),
        N.Max(w, other.w));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec4<N> Clamp(in Vec4<N> min, in Vec4<N> max) => new(
        N.Clamp(x, min.x, max.x),
        N.Clamp(y, min.y, max.y),
        N.Clamp(z, min.z, max.z),
        N.Clamp(w, min.w, max.w));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly N Multi() => x * y * z * w;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object? obj) => obj is Vec4<N> vec4 && Equals(vec4);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(in Vec4<N> other) => (this == other).AllTrue;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly string ToString() => $"({x}, {y}, {z}, {w})";
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode() => HashCode.Combine(x, y, z, w);
    #endregion FUNCTIONS
    #region MATH_OPERATORS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> operator -(in Vec4<N> v) => new(-v.x, -v.y, -v.z, -v.w);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> operator +(in Vec4<N> v1, in Vec4<N> v2) => new(
        v1.x + v2.x,
        v1.y + v2.y,
        v1.z + v2.z,
        v1.w + v2.w);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> operator -(in Vec4<N> v1, in Vec4<N> v2) => new(
        v1.x - v2.x,
        v1.y - v2.y,
        v1.z - v2.z,
        v1.w - v2.w);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> operator *(in Vec4<N> v1, in Vec4<N> v2) => new(
        v1.x * v2.x,
        v1.y * v2.y,
        v1.z * v2.z,
        v1.w * v2.w);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> operator /(in Vec4<N> v1, in Vec4<N> v2) => new(
        v1.x / v2.x,
        v1.y / v2.y,
        v1.z / v2.z,
        v1.w / v2.w);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> operator %(in Vec4<N> v1, in Vec4<N> v2) => new(
        v1.x % v2.x,
        v1.y % v2.y,
        v1.z % v2.z,
        v1.w % v2.w);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> operator +(in Vec4<N> v, in N n) => new(
        v.x + n,
        v.y + n,
        v.z + n,
        v.w + n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> operator -(in Vec4<N> v, in N n) => new(
        v.x - n,
        v.y - n,
        v.z - n,
        v.w - n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> operator *(in Vec4<N> v, in N n) => new(
        v.x * n,
        v.y * n,
        v.z * n,
        v.w * n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> operator /(in Vec4<N> v, in N n) => new(
        v.x / n,
        v.y / n,
        v.z / n,
        v.w / n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> operator %(in Vec4<N> v, in N n) => new(
        v.x % n,
        v.y % n,
        v.z % n,
        v.w % n);
    #endregion MATH_OPERATORS
    #region LOGIC_OPERATORS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool4 operator ==(in Vec4<N> a, Vec4<N> b) => new(
        a.x == b.x,
        a.y == b.y,
        a.z == b.z,
        a.w == b.w);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool4 operator !=(in Vec4<N> a, Vec4<N> b) => new(
        a.x != b.x,
        a.y != b.y,
        a.z != b.z,
        a.w != b.w);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool4 operator >(in Vec4<N> v1, in Vec4<N> v2) => new(
        v1.x > v2.x,
        v1.y > v2.y,
        v1.z > v2.z,
        v1.w > v2.w);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool4 operator <(in Vec4<N> v1, in Vec4<N> v2) => new(
        v1.x < v2.x,
        v1.y < v2.y,
        v1.z < v2.z,
        v1.w < v2.w);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool4 operator >=(in Vec4<N> v1, in Vec4<N> v2) => new(
        v1.x >= v2.x,
        v1.y >= v2.y,
        v1.z >= v2.z,
        v1.w >= v2.w);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool4 operator <=(in Vec4<N> v1, in Vec4<N> v2) => new(
        v1.x <= v2.x,
        v1.y <= v2.y,
        v1.z <= v2.z,
        v1.w <= v2.w);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool4 operator >(in Vec4<N> v, in N n) => new(
        v.x > n,
        v.y > n,
        v.z > n,
        v.w > n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool4 operator <(in Vec4<N> v, in N n) => new(
        v.x < n,
        v.y < n,
        v.z < n,
        v.w < n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool4 operator >=(in Vec4<N> v, in N n) => new(
        v.x >= n,
        v.y >= n,
        v.z >= n,
        v.w >= n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bool4 operator <=(in Vec4<N> v, in N n) => new(
        v.x <= n,
        v.y <= n,
        v.z <= n,
        v.w <= n);
    #endregion LOGIC_OPERATORS
    #region CONVERTERS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec4<TOutput> CastTo<TOutput>() where TOutput : INumber<TOutput> => Vec4<TOutput>.CreateFrom(x, y, z, w);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> CreateFrom<TInput>(TInput x, TInput y, TInput z, TInput w)
        where TInput : INumber<TInput> => new(
        N.CreateChecked(x),
        N.CreateChecked(y),
        N.CreateChecked(z),
        N.CreateChecked(w));
    #endregion CONVERTERS
}
