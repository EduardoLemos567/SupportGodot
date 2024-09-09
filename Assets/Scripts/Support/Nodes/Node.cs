using System.Collections.Generic;

namespace Support.Nodes;

public interface IHasChildren<T> : IEnumerable<T>
{
    ReadOnlySet<T> Children { get; }
    void AddChild(in T child);
    void RemoveChild(in T child);
}

public interface IHasParent<T>
{
    public T? Parent { get; }
    public void SetParent(in T? parent);
}

public interface IHasSiblings<T>
{
    ReadOnlySet<T> Siblings { get; }
    void AddSibling(in T sibling);
    void RemoveSibling(in T sibling);
}

public abstract class Node
{
    //TODO: complete
    // handle every case, root, trunk, leaf
}