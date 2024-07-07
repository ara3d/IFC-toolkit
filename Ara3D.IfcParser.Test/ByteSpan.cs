using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Ara3D.IfcParser.Test;

public readonly unsafe struct ByteSpan
{
    public readonly byte* Ptr;
    public readonly int Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteSpan(byte* begin, byte* end)
        : this(begin, (int)(end - begin))
    {
    }

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
    public byte[] ToArray()
    {
        var r = new byte[Length];
        for (var i = 0; i < Length; ++i)
            r[i] = Ptr[i];
        return r;
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(string str)
    {
        if (str.Length != Length) return false;
        for (var i = 0; i < Length; i++)
            if (str[i] != Ptr[i])
                return false;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj)
        => obj is ByteSpan span && Equals(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
        => HashHelpers.Hash(Ptr, Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ByteSpan other)
    {
        if (other.Length != Length) return false;
        var pA = Ptr;
        var pB = other.Ptr;
        for (var i = 0; i < Length; i++)
            if (*pA++ != *pB++)
                return false;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteSpan CreatePermanent(byte[] array)
    {
        var h = GCHandle.Alloc(array);
        var p = (byte*)h.AddrOfPinnedObject();
        return new(p, array.Length);
    }
}

public static class ByteSpanExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteSpan ToByteSpanASCII(this string str)
        => ByteSpan.CreatePermanent(Encoding.ASCII.GetBytes(str));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteSpan ToByteSpan(this byte[] bytes)
        => ByteSpan.CreatePermanent(bytes);
}

public static class HashHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Combine(int h1, int h2)
    {
        var rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
        return ((int)rol5 + h1) ^ h2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int Hash(byte* ptr, int length)
    {
        const int seed = unchecked((int)0x811C9DC5);
        var hash = seed;

        var i = 0;
        while (i <= length - 4)
        {
            var value = ptr[i++]
                        | (ptr[i++] << 8)
                        | (ptr[i++] << 16)
                        | (ptr[i++] << 24);
            hash = Combine(hash, value);
        }

        while (i < length)
        {
            hash = Combine(hash, ptr[i++]);
        }

        return hash;
    }
}
    