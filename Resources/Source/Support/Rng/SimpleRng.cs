using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Support.Rng;

//NOTE: if we use struct, it wont change state on IRng extensions.
public class SimpleRng : IRng
{
    private uint state;
    public int Seed { get; }
    public SimpleRng(int seed = 0)
    {
        if (seed == 0) { seed = 1; }
        Seed = seed;
        state = (uint)seed;
        NextState();
    }
    public SimpleRng(object obj) : this(obj.GetHashCode()) { }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint NextState()
    {
        uint s = state;
        state ^= state << 15;
        state ^= state >> 19;
        state ^= state << 7;
        return s;
    }
    /// <summary>
    /// Get a random number between min and max.
    /// <br>Supports only: int, float, double.</br>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public T GetNumber<T>(T minValue, T maxValue) where T : INumber<T>
    {
        var type = typeof(T);
        if (type == typeof(int))
        {
            return T.CreateChecked(GetInt(int.CreateChecked(minValue), int.CreateChecked(maxValue)));
        }
        else if (type == typeof(float))
        {
            return T.CreateChecked(GetFloat(float.CreateChecked(minValue), float.CreateChecked(maxValue)));
        }
        else if (type == typeof(double))
        {
            return T.CreateChecked(GetDouble(double.CreateChecked(minValue), double.CreateChecked(maxValue)));
        }
        else
        {
            throw new NotSupportedException("Type T is not supported.");
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetInt(int minValue, int maxValue)
    {
        if (minValue == maxValue) { return minValue; }
        if (minValue > maxValue) { (minValue, maxValue) = (maxValue, minValue); }
        var range = (uint)(maxValue - minValue);
        return (int)(NextState() * (ulong)range >> 32) + minValue;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private float GetFloat(float minValue, float maxValue)
    {
        if (minValue == maxValue) { return minValue; }
        if (minValue > maxValue) { (minValue, maxValue) = (maxValue, minValue); }
        var nnextxtFloat = BitConverter.UInt32BitsToSingle(0x3f800000 | (NextState() >> 9)) - 1.0f;
        return minValue + (maxValue - minValue) * nnextxtFloat;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double GetDouble(double minValue, double maxValue)
    {
        if (minValue == maxValue) { return minValue; }
        if (minValue > maxValue) { (minValue, maxValue) = (maxValue, minValue); }
        var signif = ((ulong)NextState() << 20) ^ NextState();
        var next = BitConverter.UInt64BitsToDouble(0x3ff0000000000000 | signif) - 1.0;
        return minValue + (maxValue - minValue) * next;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset()
    {
        state = (uint)Seed;
        NextState();
    }
}
