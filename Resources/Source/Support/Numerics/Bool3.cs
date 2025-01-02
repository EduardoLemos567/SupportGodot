using System;
using System.Runtime.InteropServices;

namespace Support.Numerics;

[StructLayout(LayoutKind.Sequential)]
public struct Bool3 : IVectorBool
{
    public bool x;
    public bool y;
    public bool z;
    public readonly int Length => 3;
    #region SWIZZLE
    public readonly bool this[int x] => x switch
    {
        0 => this.x,
        1 => y,
        2 => z,
        _ => throw new ArgumentException("Index out of valid range")
    };
    public readonly Bool2 this[int x, int y] => new(this[x], this[y]);
    public readonly Bool3 this[int x, int y, int z] => new(this[x], this[y], this[z]);
    #endregion SWIZZLE
    public readonly bool AllTrue => x && y && z;
    public readonly bool AnyTrue => x || y || z;
    public readonly bool AllEqual => x == y == z;
    public Bool3(bool x, bool y, bool z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
