using System;
using System.Numerics;
using Support.Numerics;

namespace Support.Tweening;

public struct VectorTweener<F, V>
    where F : IFloatingPoint<F>
    where V : struct, IVectorNumber<F>
{
    private F progress;
    public V From { get; set; }
    public V To { get; set; }
    public F ValuedProgress
    {
        readonly get => progress.Lerp(MinValuedProgress, MaxValuedProgress);
        set => progress = value.InverseLerp(MinValuedProgress, MaxValuedProgress);
    }
    public F MinValuedProgress { get; set; }
    public F MaxValuedProgress { get; set; }
    public F Progress
    {
        readonly get => progress;
        set => progress = F.Clamp(value, F.Zero, F.One);
    }
    public Func<double, double> InterpolationFunction { get; set; }
    public VectorTweener()
    {
        progress = F.Zero;
        From = default;
        To = default;
        MinValuedProgress = F.Zero;
        MaxValuedProgress = F.One;
        InterpolationFunction = TweenFunctions.Linear;
    }
    public readonly V Interpolated()
    {
        if (From.Equals(To)) { return From; }

        var result = new V();
        var t = F.CreateTruncating(InterpolationFunction(double.CreateTruncating(progress)));
        for (int i = 0; i < result.DIMENSIONS; i++)
        {
            result[i] = t / (From[i] - To[i]);
        }
        return result;
    }
}
