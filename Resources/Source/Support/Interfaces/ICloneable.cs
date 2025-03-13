namespace Support;

/// <summary>
/// Implements a shallow Clone function that returns the type T.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICloneable<T>
{
    T Clone();
}