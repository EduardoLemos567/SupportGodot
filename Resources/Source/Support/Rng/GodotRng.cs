using System;
using System.Runtime.CompilerServices;
using Godot;

namespace Support.Rng;

/// <summary>
/// System representation of .NET Random continual rng.
/// </summary>
public class GodotRng : ARng
{
    private RandomNumberGenerator state = new();
    public static GodotRng GlobalState { get; set; } = new(TimeSeed);
    public GodotRng(object obj) : base(obj) { }
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
        return state.RandiRange(minValue, maxValue - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private float GetFloat(float minValue, float maxValue)
    {
        if (minValue == maxValue) { return minValue; }
        if (minValue > maxValue) { (minValue, maxValue) = (maxValue, minValue); }
        return minValue + (maxValue - minValue) * (state.Randf() - float.Epsilon);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double GetDouble(double minValue, double maxValue)
    {
        if (minValue == maxValue) { return minValue; }
        if (minValue > maxValue) { (minValue, maxValue) = (maxValue, minValue); }
        return minValue + (maxValue - minValue) * (state.Randf() - float.Epsilon);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Reset()
    {
        state.Seed = Seed;
    }
}