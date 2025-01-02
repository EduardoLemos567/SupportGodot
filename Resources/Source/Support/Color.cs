using Support.Numerics;

namespace Support;

public struct Color
{
    public readonly static Color BLACK = new(0, 0, 0, 1);
    public readonly static Color WHITE = new(1, 1, 1, 1);
    public readonly static Color RED = new(1, 0, 0, 1);
    public readonly static Color GREEN = new(0, 1, 0, 1);
    public readonly static Color BLUE = new(0, 0, 1, 1);
    public float r;
    public float g;
    public float b;
    public float a = 1;
    public Color() { }
    public Color(float r, float g, float b, float a = 1)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
    public Color MixAdditive(in Color color, float alpha = 1.0f)
    {
        return new()
        {
            r = (r + color.r) / 2,
            g = (g + color.g) / 2,
            b = (b + color.b) / 2,
            a = alpha
        };
    }
    public Color MixSubtractive(in Color color, float alpha = 1.0f)
    {
        return new()
        {
            r = r - (r - color.r) / 2,
            g = g - (g - color.g) / 2,
            b = b - (b - color.b) / 2,
            a = alpha
        };
    }
    public static implicit operator Vec4<float>(in Color color) => new(color.r, color.g, color.b, color.a);
    public static implicit operator Color(in Vec4<float> vec4) => new(vec4.x, vec4.y, vec4.z, vec4.w);
}
