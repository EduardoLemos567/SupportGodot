using System;
using System.Numerics;
using Support.Numerics;

namespace Support.Tweening;

public static class InterpolationFunctions
{
    public static float Linear(float progress)
    {
        if (progress < 0) { return 0; } else if (progress > 1) { return 1; }
        return progress;
    }
    public static float EaseIn(float progress)
    {
        if (progress < 0) { return 0; } else if (progress > 1) { return 1; }
        return progress * progress * progress;
    }
}

public struct VectorTweener<N, V> where N: INumber<N> where V: IVectorNumber<N>, new()
{
    private float progress;
    public V From { get; }
    public V To { get; }
    public float Progress {
        readonly get => progress; 
        set => progress = float.Clamp(value, 0, 1);
    }
    public float MinProgress { get; }
    public float MaxProgress { get; }
    public float NormalizedProgress
    {
        readonly get => float.Lerp(MinProgress, MaxProgress, progress); 
        set => progress = value.InverseLerp(MinProgress, MaxProgress);
    }
    public V Interpolation
    {
        get
        {
            var result = new V();
            for (int i = 0; i < result.DIMENSIONS; i++)
            {
                result[i] = N.Lerp(From[i], To[i], NormalizedProgress);
            }
            return result;
        }
    }
    public Func<float, float> InterpolationFunction { get; }
    public VectorTweener(V from, V to, float minProgress = 0, float maxProgress = 1, Func<float, float>? interpolationFunction = null)
    {
        From = from;
        To = to;
        MinProgress = minProgress;
        MaxProgress = maxProgress;
        InterpolationFunction = interpolationFunction ?? InterpolationFunctions.Linear;
    }
}