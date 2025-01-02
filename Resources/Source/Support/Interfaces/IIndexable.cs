namespace Support
{
    public interface IIndexable<T>
    {
        T this[int index] { get; set; }
        int Count { get; }
    }
}
