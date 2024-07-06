using Ara3D.Utils;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Ara3D.Logging;

namespace Ara3D.IfcParser.Test;

public unsafe class StepDocument : IDisposable
{
    public readonly StepTokens Tokens;
    public readonly int[] TokenIndices;
    public readonly int NumTokens;
    public readonly StepRawRecord[] Records;
    public readonly int NumRecords;
    public readonly byte* DataStart;
    public readonly byte* DataEnd;
    public readonly long Length;
    private GCHandle _handle;
    public StepIdLookup Lookup;

    public string FirstError;

    public StepDocument(FilePath filePath, ILogger logger = null)
    {
        logger ??= new Logger(LogWriter.DebugWriter, "Ara 3D Step Document Loader");

        logger.Log($"Loading {filePath.GetFileSizeAsString()} of data from {filePath.GetFileName()}");
        var _buffer = filePath.ReadAllBytes();

        logger.Log("Pinning data");
        Length = _buffer.Length;
        _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
        DataStart = (byte*)_handle.AddrOfPinnedObject().ToPointer();
        DataEnd = DataStart + Length;

        logger.Log("Creating tokens");
        Tokens = StepTokenizer.CreateTokens(DataStart, (int)Length)
                 ?? throw new Exception("Tokenization failed");
        TokenIndices = Tokens.Tokens;
        NumTokens = TokenIndices.Length;
        Records = Tokens.Records;
        NumRecords = Records.Length;
        logger.Log($"Created {NumTokens} tokens");

        logger.Log($"Creating records");
        fixed (StepRawRecord* pArray = &Records[0])
        {
            for (var i = 0; i < NumRecords; i++)
            {
                var ptr = pArray + i;
                var span = GetTokenSpan(ptr->BeginToken);
                ptr->Index = i;
                ptr->Id = span.Skip(1).ToInt();
            }
        }
        logger.Log($"Created {NumRecords} records");

        logger.Log("Creating lookup");
        Lookup = new(Records);

        logger.Log($"Completed creation of STEP document from {filePath.GetFileName()}");
    }

    public void Dispose()
    {
        _handle.Free();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteSpan GetSpan(byte* begin, byte* end)
        => new(begin, (int)(end - begin));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetTokenLength(int index)
        => (int)(GetTokenPtr(index + 1) - GetTokenPtr(index));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte* GetTokenPtr(int index)
        => DataStart + TokenIndices[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TokenType GetTokenType(int index)
        => StepTokenizer.LookupToken(*GetTokenPtr(index));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteSpan GetTokenSpan(int index)
    {
        var a = TokenIndices[index];
        var b = TokenIndices[index + 1];
        return new(DataStart + a, b - a);
    }
}