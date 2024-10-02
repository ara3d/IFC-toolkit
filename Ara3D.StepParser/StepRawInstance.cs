using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Ara3D.Buffers;

namespace Ara3D.StepParser
{
    /// <summary>
    /// Contains information about where an instance is within a file.
    /// </summary>  
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly struct StepRawInstance(uint id, ByteSpan type, int lineIndex, int index)
    {
        public readonly uint Id = id;
        public readonly ByteSpan Type = type;
        public readonly int LineIndex = lineIndex;
        public readonly int Index = index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid()
            => Id > 0;

        public override string ToString()
            => $"{Id} = {Type}";
    }
}