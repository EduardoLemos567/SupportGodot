using System;
using System.Runtime.InteropServices;

namespace Support.Numerics;

[StructLayout(LayoutKind.Sequential)]
public struct Bool4 : IVectorBool
{
    public bool x;
    public bool y;
    public bool z;
    public bool w;
    public readonly int DIMENSIONS => 4;
    #region SWIZZLE
    public readonly bool this[int x] => x switch
    {
        0 => this.x,
        1 => y,
        2 => z,
        3 => w,
        _ => throw new ArgumentException("Index out of valid range")
    };
    public readonly Bool2 this[int x, int y] => new(this[x], this[y]);
    public readonly Bool3 this[int x, int y, int z] => new(this[x], this[y], this[z]);
    public readonly Bool4 this[int x, int y, int z, int w] => new(this[x], this[y], this[z], this[w]);
    #endregion SWIZZLE
    public readonly bool AllTrue => x && y && z && w;
    public readonly bool AnyTrue => x || y || z || w;
    public readonly bool AllEqual => x == y == z == w;
    public Bool4(bool x, bool y, bool z, bool w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
    public override readonly string ToString() => $"({x}, {y}, {z}, {w})";
}