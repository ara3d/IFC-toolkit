using System.Runtime.CompilerServices;
using System.Text;

namespace Ara3D.IfcParser.Test;

public readonly unsafe struct ByteSpan
{
    public readonly byte* Ptr;
    public readonly int Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteSpan(byte* ptr, int length)
    {
        Ptr = ptr; 
        Length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte First()
        => *Ptr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Last()
        => Ptr[Length - 1];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte* End()
        => Ptr + Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte* Begin()
        => Ptr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsBefore(ByteSpan other)
        => End() <= other.Begin(); 

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteSpan(byte* begin, byte* end)
        : this(begin, (int)(end - begin))
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ToSpan()
        => new(Ptr, Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
        => Encoding.ASCII.GetString(Ptr, Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ToDouble()
        => double.Parse(ToSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ToInt()
        => int.Parse(ToSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte At(int index)
        => Ptr[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteSpan Slice(int from, int count)
        => new(Ptr + from, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteSpan Skip(int count)
        => Slice(count, Length - count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteSpan Take(int count)
        => new(Ptr, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteSpan Drop(int count)
        => new(Ptr, Length - count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteSpan Trim(int before, int after)
        => new(Ptr + before, Length - before - after);
}