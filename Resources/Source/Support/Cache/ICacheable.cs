namespace Support.Cache;

public interface ICacheable
{
    /// <summary>
    /// Object is cached.
    /// </summary>
    public bool IsCached { get; set; }
}