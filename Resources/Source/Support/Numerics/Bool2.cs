using System;
using System.Runtime.InteropServices;

namespace Support.Numerics;

[StructLayout(LayoutKind.Sequential)]
public struct Bool2 : IVectorBool
{
    public bool x;
    public bool y;
    public readonly int DIMENSIONS => 2;
    #region SWIZZLE
    public readonly bool this[int x] => x switch
    {
        0 => this.x,
        1 => y,
        _ => throw new ArgumentException("Index out of valid range")
    };
    public readonly Bool2 this[int x, int y] => new(this[x], this[y]);
    #endregion SWIZZLE
    public readonly bool AllTrue => x && y;
    public readonly bool AnyTrue => x || y;
    public readonly bool AllEqual => x == y;
    public Bool2(bool x, bool y)
    {
        this.x = x;
        this.y = y;
    }
}
