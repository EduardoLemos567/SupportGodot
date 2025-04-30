using System;
using System.Numerics;
using Support.Numerics;

namespace Support.Tweening;

public struct NumberTweener<F>
    where F : struct, IFloatingPoint<F>
{
    private F progress;
    public F From { get; set; }
    public F To { get; set; }
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
    public NumberTweener()
    {
        progress = F.Zero;
        From = default;
        To = default;
        MinValuedProgress = F.Zero;
        MaxValuedProgress = F.One;
        InterpolationFunction = TweenFunctions.Linear;
    }
    public readonly F Interpolated()
    {
        if (From.Equals(To)) { return From; }

        var t = F.CreateTruncating(InterpolationFunction(double.CreateTruncating(progress)));

        return t.Lerp(From, To);
    }
}
