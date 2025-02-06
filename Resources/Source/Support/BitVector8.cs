using System;

namespace Support;

public struct BitVector8 : IIndexable<bool>, IEquatable<BitVector8>
{
    private const int COUNT = 8;
    public byte Value { get; set; }
    public bool IsAllFalse
    {
        readonly get => Value == 0;
        set { if (value) { this.Value = 0; } }
    }
    public readonly int Count => COUNT;
    public bool this[int index]
    {
        readonly get => index is < 0 or >= COUNT ?
            throw new ArgumentException("Index must be between [0..7]")
            : ((Value >> index) & 1) == 1;
        set
        {
            if (index is < 0 or >= COUNT) { throw new ArgumentException("Index must be between [0..7]"); }
            this.Value = (byte)(value ? (this.Value | (1 << index)) : (this.Value & (this.Value ^ (1 << index))));
        }
    }
    public BitVector8(byte data) => Value = data;
    public BitVector8(params bool[] data)
    {
        if (data.Length is 0 or >= COUNT) { throw new ArgumentException("data.Length must be between [1..8]"); }
        Value = 0;
        for (var i = 0; i < data.Length; i++)
        { this[i] = data[i]; }
    }
    public BitVector8(params int[] data)
    {
        if (data.Length is 0 or >= COUNT) { throw new ArgumentException("data.Length must be between [1..8]"); }
        Value = 0;
        for (var i = 0; i < data.Length; i++)
        { this[data[i]] = true; }
    }
    public readonly bool BinaryEqual(in byte bin) => (Value & bin) == bin;
    public void BinarySet(in byte bin) => Value |= bin;
    public readonly int TrueCount()
    {
        var count = 0;
        for (var i = 0; i < COUNT; i++)
        {
            count += (Value >> i) & 1;
        }
        return count;
    }
    public readonly bool TrueCountGreaterThan(int count)
    {
        for (var i = 0; i < COUNT; i++)
        {
            count -= (Value >> i) & 1;
            if (count < 0) { return true; }
        }
        return false;
    }
    public override readonly bool Equals(object? other) => other is BitVector8 vector && Equals(vector);
    public readonly bool Equals(BitVector8 other) => Value == other.Value;
    public override readonly int GetHashCode() => Value.GetHashCode();
    public override readonly string ToString() => Value.ToString();
    public static bool operator ==(in BitVector8 one, in BitVector8 other) => one.Value == other.Value;
    public static bool operator !=(in BitVector8 one, in BitVector8 other) => one.Value != other.Value;
}