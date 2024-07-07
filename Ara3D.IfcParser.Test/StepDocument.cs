using System.Collections.Concurrent;
using Ara3D.Utils;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Ara3D.Logging;

namespace Ara3D.IfcParser.Test;

public unsafe class StepDocument : IDisposable
{
    public readonly StepTokens Tokens;
    public readonly byte*[] TokenPtrs;
    public readonly int NumTokens;
    public readonly StepRawRecord[] RawRecords;
    public readonly int NumRecords;
    public readonly byte* DataStart;
    public readonly byte* DataEnd;
    public readonly long Length;
    private GCHandle _handle;
    public StepIdLookup Lookup;
        
    public StepDocument(FilePath filePath, ILogger logger = null)
    {
        logger ??= new Logger(LogWriter.ConsoleWriter, "Ara 3D Step Document Loader");

        logger.Log($"Loading {filePath.GetFileSizeAsString()} of data from {filePath.GetFileName()}");
        var _buffer = filePath.ReadAllBytes();

        logger.Log("Pinning data");
        Length = _buffer.Length;
        _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
        DataStart = (byte*)_handle.AddrOfPinnedObject().ToPointer();
        DataEnd = DataStart + Length;

        Tokens = StepTokenizer.CreateTokens(DataStart, DataEnd, logger)
                 ?? throw new Exception("Tokenization failed");

        TokenPtrs = Tokens.Tokens;
        NumTokens = TokenPtrs.Length;
        RawRecords = Tokens.Entities;
        NumRecords = RawRecords.Length;
        logger.Log($"Created {NumTokens} tokens");

        logger.Log($"Creating records");
        fixed (StepRawRecord* pArray = &RawRecords[0])
        {
            for (var i = 0; i < NumRecords; i++)
            {
                var ptr = pArray + i;
                var begin = GetTokenPtr(ptr->BeginToken) + 1;
                var length = GetTokenPtr(ptr->BeginToken + 1) - begin;
                var tmp = new ReadOnlySpan<byte>(begin, (int)length);
                var test = int.TryParse(tmp, out ptr->Id);
                if (!test)
                    throw new Exception($"Failed to record ID at token {ptr->BeginToken}");
            }
        }
        logger.Log($"Created {NumRecords} records");

        logger.Log("Creating lookup");
        Lookup = new(RawRecords);
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
        => TokenPtrs[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TokenType GetTokenType(int index)
        => StepTokenizer.LookupToken(*GetTokenPtr(index));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteSpan GetTokenSpan(int index)
        => GetSpan(GetTokenPtr(index), GetTokenPtr(index+1));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StepRecord GetRecord(int index)
        => CreateRecord(RawRecords[index]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StepRecord CreateRecord(StepRawRecord location)
    {
        var defTokenIndex = location.BeginToken + 2;
        var val = StepFactory.Create(this, ref defTokenIndex, location.EndToken);
        return new StepRecord(location.Id, (StepInstance)val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<StepRecord> GetRecords()
        => RawRecords.Select(CreateRecord);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEntity(int recordIndex, ByteSpan entityType)
    {
        var rr = RawRecords[recordIndex];
        var span = GetTokenSpan(rr.BeginToken + 2);
        return span.Equals(entityType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<StepValue> GetAttributes(int recordIndex)
    {
        var rec = GetRecord(recordIndex);
        return rec.Value.Attributes.Values;
    }
}