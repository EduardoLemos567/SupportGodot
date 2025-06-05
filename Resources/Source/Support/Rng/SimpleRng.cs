using System;
using System.Runtime.CompilerServices;

namespace Support.Rng;

//NOTE: if we use struct, it wont change state on IRng extensions.
public class SimpleRng : ARng
{
    private uint state;
    public static SimpleRng GlobalState { get; set; } = new(TimeSeed);
    public SimpleRng(object? seed = null) : base(seed) { }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint NextState()
    {
        uint s = state;
        state = MurmurHash3(state);
        return s;
    }
    public static uint MurmurHash3(uint x)
    {
        x ^= x >> 16;
        x *= 0x85ebca6bu;
        x ^= x >> 13;
        x *= 0xc2b2ae35u;
        x ^= x >> 16;
        return x;
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
    public override T GetNumber<T>(T minValue, T maxValue)
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
        var nextFloat = BitConverter.UInt32BitsToSingle(0x3f800000 | (NextState() >> 9)) - 1.0f;
        return minValue + (maxValue - minValue) * nextFloat;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double GetDouble(double minValue, double maxValue)
    {
        if (minValue == maxValue) { return minValue; }
        if (minValue > maxValue) { (minValue, maxValue) = (maxValue, minValue); }
        var signif = ((ulong)NextState() << 20) ^ NextState();
        var nextDouble = BitConverter.UInt64BitsToDouble(0x3ff0000000000000 | signif) - 1.0;
        return minValue + (maxValue - minValue) * nextDouble;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Reset()
    {
        state = Seed;
        NextState();
    }
}
