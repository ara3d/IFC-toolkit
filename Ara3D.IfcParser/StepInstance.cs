using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Ara3D.Buffers;

namespace Ara3D.IfcParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct StepInstance
{
    public readonly int Id;
    public readonly ByteSpan Type;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StepInstance(int id, ByteSpan type)
    {
        Id = id;
        Type = type;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
        => Id > 0;

    public override string ToString()
        => $"{Id} = {Type}";
}