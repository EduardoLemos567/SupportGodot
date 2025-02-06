using System;
using System.Numerics;

namespace Support.Rng;

/// <summary>
/// Allow different implementations of RNGs.
/// </summary>
public interface IRng
{
    /// <summary>
    /// Time based seed, counted since OS startup.
    /// </summary>
    static int TimeSeed => Environment.TickCount;
    int Seed { get; }
    T GetNumber<T>(T min, T max) where T : INumber<T>;
    void Reset();
}