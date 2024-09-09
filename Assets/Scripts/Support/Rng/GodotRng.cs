namespace Support.Rng;

// GODOT RNG is garbage, it uses inclusive endings, which is bad to pick items on a list, as it will try to pick the length value of the list.

#if false
public class GodotRng : IRng
{
    private RandomNumberGenerator state;
    public static GodotRng GlobalState { get; set; } = new(IRng.TimeSeed);
    public int Seed { get; }
    public GodotRng(int seed = 0)
    {
        if (seed == 0) { seed = 1; }
        Seed = seed;
        state = new() { Seed = (ulong)Seed };
    }
    public GodotRng(object obj) : this(obj.GetHashCode()) { }
    /// <summary>
    /// Get a random number between min and max.
    /// <br>Supports only: int, float, double.</br>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public T GetNumber<T>(T min, T max) where T : INumber<T>
    {
        var type = typeof(T);
        if (type == typeof(int))
        {
            return T.CreateChecked(GetInt(int.CreateChecked(min), int.CreateChecked(max)));
        }
        else if (type == typeof(float))
        {
            return T.CreateChecked(GetFloat(float.CreateChecked(min), float.CreateChecked(max)));
        }
        else if (type == typeof(double))
        {
            return T.CreateChecked(GetDouble(double.CreateChecked(min), double.CreateChecked(max)));
        }
        else
        {
            throw new NotSupportedException("Type T is not supported.");
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetInt(int minValue, int maxValue) => minValue == maxValue ? minValue : state.RandiRange(minValue, maxValue);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private float GetFloat(float minValue, float maxValue) => minValue == maxValue ? minValue : state.RandfRange(minValue, maxValue);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double GetDouble(double minValue, double maxValue) => minValue == maxValue ? minValue : minValue + (maxValue - minValue) * state.Randf();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset() => state = new() { Seed = (ulong)Seed };
}
#endif