using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Ara3D.Spans;

namespace Ara3D.IfcParser;

public class SimdIfcTokenizer
{
    public static readonly Vector256<byte> Comma = Vector256.Create((byte)',');
    public static readonly Vector256<byte> NewLine = Vector256.Create((byte)'\n');
    public static readonly Vector256<byte> StartGroup = Vector256.Create((byte)'(');
    public static readonly Vector256<byte> EndGroup = Vector256.Create((byte)')');
    public static readonly Vector256<byte> Definition = Vector256.Create((byte)'=');
    public static readonly Vector256<byte> Quote = Vector256.Create((byte)'\'');
    public static readonly Vector256<byte> Id = Vector256.Create((byte)'#');
    public static readonly Vector256<byte> SemiColon = Vector256.Create((byte)';');
    public static readonly Vector256<byte> Unassigned = Vector256.Create((byte)'*');

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ComputeLines(in Vector256<byte> v, ref int lineIndex, List<int> lineIndices) 
    {
        var r = Avx2.CompareEqual(v, NewLine);
        var mask = (uint)Avx2.MoveMask(r);
        if (mask == 0)
        {
            lineIndex += 32;
            return;
        }

        // Fully unrolled handling of each bit
        if ((mask & 0x00000001) != 0) lineIndices.Add(lineIndex);
        if ((mask & 0x00000002) != 0) lineIndices.Add(lineIndex + 1);
        if ((mask & 0x00000004) != 0) lineIndices.Add(lineIndex + 2);
        if ((mask & 0x00000008) != 0) lineIndices.Add(lineIndex + 3);
        if ((mask & 0x00000010) != 0) lineIndices.Add(lineIndex + 4);
        if ((mask & 0x00000020) != 0) lineIndices.Add(lineIndex + 5);
        if ((mask & 0x00000040) != 0) lineIndices.Add(lineIndex + 6);
        if ((mask & 0x00000080) != 0) lineIndices.Add(lineIndex + 7);
        if ((mask & 0x00000100) != 0) lineIndices.Add(lineIndex + 8);
        if ((mask & 0x00000200) != 0) lineIndices.Add(lineIndex + 9);
        if ((mask & 0x00000400) != 0) lineIndices.Add(lineIndex + 10);
        if ((mask & 0x00000800) != 0) lineIndices.Add(lineIndex + 11);
        if ((mask & 0x00001000) != 0) lineIndices.Add(lineIndex + 12);
        if ((mask & 0x00002000) != 0) lineIndices.Add(lineIndex + 13);
        if ((mask & 0x00004000) != 0) lineIndices.Add(lineIndex + 14);
        if ((mask & 0x00008000) != 0) lineIndices.Add(lineIndex + 15);
        if ((mask & 0x00010000) != 0) lineIndices.Add(lineIndex + 16);
        if ((mask & 0x00020000) != 0) lineIndices.Add(lineIndex + 17);
        if ((mask & 0x00040000) != 0) lineIndices.Add(lineIndex + 18);
        if ((mask & 0x00080000) != 0) lineIndices.Add(lineIndex + 19);
        if ((mask & 0x00100000) != 0) lineIndices.Add(lineIndex + 20);
        if ((mask & 0x00200000) != 0) lineIndices.Add(lineIndex + 21);
        if ((mask & 0x00400000) != 0) lineIndices.Add(lineIndex + 22);
        if ((mask & 0x00800000) != 0) lineIndices.Add(lineIndex + 23);
        if ((mask & 0x01000000) != 0) lineIndices.Add(lineIndex + 24);
        if ((mask & 0x02000000) != 0) lineIndices.Add(lineIndex + 25);
        if ((mask & 0x04000000) != 0) lineIndices.Add(lineIndex + 26);
        if ((mask & 0x08000000) != 0) lineIndices.Add(lineIndex + 27);
        if ((mask & 0x10000000) != 0) lineIndices.Add(lineIndex + 28);
        if ((mask & 0x20000000) != 0) lineIndices.Add(lineIndex + 29);
        if ((mask & 0x40000000) != 0) lineIndices.Add(lineIndex + 30);
        if ((mask & 0x80000000) != 0) lineIndices.Add(lineIndex + 31);

        // Update lineIndex to the next starting position
        lineIndex += 32;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public readonly struct StepLineData
    {
        public readonly ByteSpan EntityType;
        public readonly int Id;

        public StepLineData(int id, ByteSpan entityType)
        {
            Id = id;
            EntityType = entityType;
        }
    }

    public static unsafe StepLineData ParseLine(byte* ptr, int i, int end)
    {
        var cnt = end - i;
        const int MIN_LINE_LENGTH = 5;
        if (cnt < MIN_LINE_LENGTH) return default;
        
        // Parse the ID 
        if (ptr[i++] != '#') 
            return default;
        var idStart = i;
        while (i < end)
        {
            if (ptr[i] < '0' || ptr[i] > '9') 
                break;
            i++;
        }
        var idSpan = new ByteSpan(ptr + idStart, ptr + i);
        var id = int.Parse(idSpan.ToSpan());

        // Skip separator ('=')
        while (i < end)
        {
            if (ptr[i] != (byte)' ' && ptr[i] != (byte)'=')
                break;
            i++;
        }

        // Parse the entity type
        var entityStart = i;
        while (i < end)
        {
            if (ptr[i] < 'A' || ptr[i] > 'Z')
                break;
            i++;
        }
        

        var entityType = new ByteSpan(ptr + entityStart, ptr + i);
        return new(id, entityType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint DelimiterMask(in Vector256<byte> v, out int bits)
    {
        var r = Avx2.CompareEqual(v, NewLine);
        r = Avx2.Or(r, Avx2.CompareEqual(v, Comma));
        r = Avx2.Or(r, Avx2.CompareEqual(v, StartGroup));
        r = Avx2.Or(r, Avx2.CompareEqual(v, EndGroup));
        r = Avx2.Or(r, Avx2.CompareEqual(v, Definition));
        r = Avx2.Or(r, Avx2.CompareEqual(v, Quote));
        r = Avx2.Or(r, Avx2.CompareEqual(v, Id));
        r = Avx2.Or(r, Avx2.CompareEqual(v, SemiColon));
        r = Avx2.Or(r, Avx2.CompareEqual(v, Unassigned));

        var mask = (uint)Avx2.MoveMask(r);
        bits = BitOperations.PopCount(mask);
        return mask;
    }
}