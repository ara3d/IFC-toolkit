using System.IO.MemoryMappedFiles;
using Ara3D.Utils;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Ara3D.IfcParser.Test;

public unsafe class StepDocument : IDisposable
{
    public readonly byte*[] Tokens;
    public readonly byte* DataStart;
    public readonly byte* DataEnd;
    public readonly long Length;

    private readonly byte[] _buffer;
    private GCHandle _handle;

    public StepDocument(FilePath filePath)
    {
        _buffer = filePath.ReadAllBytes();
        Length = _buffer.Length;
        _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
        DataStart = (byte*)_handle.AddrOfPinnedObject().ToPointer();
        DataEnd = DataStart + Length;
        Tokens = StepTokenizer.CreateTokens(DataStart, DataEnd);
    }

    public void Dispose()
    {
        _handle.Free();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetTokenLength(int index)
        => (int)(GetTokenPtr(index + 1) - GetTokenPtr(index));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte* GetTokenPtr(int index)
        => Tokens[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetNumTokens()
        => Tokens.Length - 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TokenType GetTokenType(int index)
        => StepTokenizer.LookupToken(*GetTokenPtr(index));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetTokenString(int index)
        => Encoding.ASCII.GetString(GetTokenPtr(index), GetTokenLength(index));
}