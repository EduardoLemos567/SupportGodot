namespace Support.Numerics;

public struct Transform3D
{
    public Vec3<float> translation;
    public Quaternion<float> rotation;
    public Vec3<float> scale = Vec3<float>.One;
    public Transform3D() { }
}
