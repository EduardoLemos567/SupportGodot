using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Support.Rng;

/// <summary>
/// System representation of .NET Random continual rng.
/// </summary>
public class SystemRng : IRng
{
    private Random state;
    public static SystemRng GlobalState { get; set; } = new(IRng.TimeSeed);
    public int Seed { get; }
    public SystemRng(int seed = 0)
    {
        if (seed == 0) { seed = 1; }
        Seed = seed;
        state = new(seed);
    }
    public SystemRng(object obj) : this(obj.GetHashCode()) { }
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
        return state.Next(minValue, maxValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private float GetFloat(float minValue, float maxValue)
    {
        if (minValue == maxValue) { return minValue; }
        if (minValue > maxValue) { (minValue, maxValue) = (maxValue, minValue); }
        return minValue + (maxValue - minValue) * state.NextSingle();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double GetDouble(double minValue, double maxValue)
    {
        if (minValue == maxValue) { return minValue; }
        if (minValue > maxValue) { (minValue, maxValue) = (maxValue, minValue); }
        return minValue + (maxValue - minValue) * state.NextDouble();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset() => state = new(Seed);
}