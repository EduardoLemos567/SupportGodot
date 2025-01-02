namespace Support.Numerics;

public struct Transform2D
{
    public Vec2<float> translation;
    public float rotation;
    public Vec2<float> scale = Vec2<float>.One;
    public Transform2D() { }
}
