using System;
using System.Numerics;
using Support.Numerics;

namespace Support.Tweening;

public struct ProgressTweener<F>
    where F : struct, IFloatingPoint<F>
{
    private F progress;
    public F ValuedProgress
    {
        readonly get => progress.LerpBetween(MinValuedProgress, MaxValuedProgress);
        set => progress = value.InverseLerpBetween(MinValuedProgress, MaxValuedProgress);
    }
    public F MinValuedProgress { get; set; }
    public F MaxValuedProgress { get; set; }
    public F Progress
    {
        readonly get => progress;
        set => progress = F.Clamp(value, F.Zero, F.One);
    }
    public Func<double, double> Function { get; set; }
    public ProgressTweener()
    {
        progress = F.Zero;
        MinValuedProgress = F.Zero;
        MaxValuedProgress = F.One;
        Function = TweenFunctions.Linear;
    }
    public readonly F Interpolated()
    {
        return F.CreateTruncating(Function(double.CreateTruncating(progress)));
    }
}
