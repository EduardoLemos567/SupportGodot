using System;

namespace Support;

public interface IReordeable<E> where E : Enum
{
    public E Order { get; }
}