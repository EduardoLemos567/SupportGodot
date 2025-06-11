using System;
using Support.Numerics;
using Support.Rng;

namespace Support.Geometrics;

/// <summary>
/// This will represent a radian angle and all convertion related tasks.
/// </summary>
public struct RadianAngle : IConstraintable
{
    private const float RAD_TO_DEGREE = 180 / MathF.PI;
    private const float DEGREE_TO_RAD = MathF.PI / 180;
    public static readonly Range<float> RADIAN_RANGE = new(0, 2 * MathF.PI);
    public static readonly Range<float> DEGREE_RANGE = new(0, 360);
    private float _radian;
    public float Radian
    {
        readonly get => _radian;
        set
        {
            _radian = value;
            EnforceConstraint();
        }
    }
    public float Degree
    {
        readonly get => ToDegree(Radian);
        set => Radian = FromDegree(value);
    }
    public Vec2<float> Delta
    {
        readonly get => new(MathF.Cos(Radian), MathF.Sin(Radian));
        set => Radian = MathF.Atan2(value.y, value.x);
    }
    /// <summary>
    /// In this case, Delta will point to Top/North on zero, instead of Right/East.
    /// </summary>
    public Vec2<float> NorthDelta
    {
        readonly get => new(MathF.Sin(Radian), MathF.Cos(Radian));
        set => Radian = MathF.Atan2(value.x, value.y);
    }
    public readonly RadianAngle Opposite => this + MathF.PI;
    /// <summary>
    /// Limit the radian value to ]-PI..PI]
    /// </summary>
    public readonly float LimitedTo1PI
    {
        get
        {
            float radian = this;
            return radian > RADIAN_RANGE.Mid ? radian - RADIAN_RANGE.Max : radian;
        }
    }
    /// <summary>
    /// Limit the radian value to ]-180..180]
    /// </summary>
    public readonly float LimitedDegreeTo180
    {
        get
        {
            var degree = Degree;
            return degree > DEGREE_RANGE.Mid ? degree - DEGREE_RANGE.Max : degree;
        }
    }
    public RadianAngle(float radian)
    {
        _radian = radian;
        EnforceConstraint();
    }
    public void EnforceConstraint() => _radian = RADIAN_RANGE.PositiveModulo(_radian);
    public static RadianAngle RandomAngle(in ARng rng) => new(rng.GetPointIn(RADIAN_RANGE));
    public static RadianAngle operator +(in RadianAngle range, float value) => new(range.Radian + value);
    public static RadianAngle operator -(in RadianAngle range, float value) => new(range.Radian - value);
    public static RadianAngle operator *(in RadianAngle range, float value) => new(range.Radian * value);
    public static RadianAngle operator /(in RadianAngle range, float value) => new(range.Radian / value);
    public static implicit operator float(in RadianAngle angle) => angle.Radian;
    private static float ToDegree(float radian) => RADIAN_RANGE.PositiveModulo(radian) * RAD_TO_DEGREE;
    private static float FromDegree(float degree) => DEGREE_RANGE.PositiveModulo(degree) * DEGREE_TO_RAD;
}
