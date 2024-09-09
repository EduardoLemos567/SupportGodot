namespace Support.Numerics;

public interface IVectorBool : IVector
{
    bool this[int index] { get; }
}
