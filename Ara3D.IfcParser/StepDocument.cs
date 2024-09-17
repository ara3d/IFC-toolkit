using System.Runtime.Intrinsics;
using Ara3D.Buffers;
using Ara3D.Buffers.Modern;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.IfcParser;

public unsafe class StepDocument : IDisposable
{
    public readonly FilePath FilePath;
    public readonly byte* DataStart;
    public readonly AlignedMemory Data;

    /// <summary>
    /// This is a list of raw step instance information.
    /// Each one has only a type and an ID.
    /// </summary>
    public readonly StepInstance[] Instances;

    /// <summary>
    /// This gives us a fast way to look up a StepInstance by their ID
    /// </summary>
    public readonly StepInstanceLookup Lookup;

    /// <summary>
    /// This tells us the byte offset of the start of each line in the file
    /// </summary>
    public readonly List<int> LineOffsets;

    public StepDocument(FilePath filePath, ILogger logger = null)
    {
        FilePath = filePath;
        logger ??= Logger.Null;

        logger.Log($"Loading {filePath.GetFileSizeAsString()} of data from {filePath.GetFileName()}");
        Data = AlignedMemoryReader.ReadAllBytes(filePath);
        DataStart = Data.BytePtr;

        logger.Log($"Computing the start of each line");
        // NOTE: this estimates that the average line length is more than 16 characters. 
        // This is a reasonable estimate. Only very degenerate files would not meet that. 
        var cap = Data.NumBytes / 16;
        LineOffsets = new List<int>(cap);
        
        // We are going to report the beginning of the lines, while the "ComputeLines" function
        // will compute the ends of lines.
        var currentLine = 1;
        for (var i=0; i < Data.NumVectors; i++)
        {
            StepLineParser.ComputeLines(
                ((Vector256<byte>*)Data.BytePtr)[i], ref currentLine, LineOffsets);
        }
        logger.Log($"Found {LineOffsets.Count} lines");

        logger.Log($"Creating instance records");
        Instances = new StepInstance[LineOffsets.Count];
        var cntValid = 0;

        // NOTE: this could be parallelized
        // NOTE: if a string has a newline in it, then they will need to be pieced together.
        // Possibly I can detect this by looking for an odd number of apostrophes. 
        for (var i = 0; i < Instances.Length - 1; i++)
        {
            var lineStart = LineOffsets[i];
            var lineEnd = LineOffsets[i + 1];
            var inst = StepLineParser.ParseLine(DataStart, lineStart, lineEnd);
            Instances[i] = inst;
            if (inst.IsValid())
                cntValid++;
        }

        logger.Log($"Found {cntValid} instances");

        logger.Log("Creating instance ID lookup");
        Lookup = new(Instances);
        logger.Log($"Completed creation of STEP document from {filePath.GetFileName()}");
    }

    public void Dispose() => Data.Dispose();

    public StepInstance[] GetInstances() => Instances;

    public IEnumerable<StepInstance> GetInstances(ByteSpan type) =>
        Instances.Where(inst 
            => inst.Type.Equals(type));

    public IEnumerable<StepInstance> GetInstances(string type) =>
        type.WithSpan(span => 
            Instances.Where(inst => 
                inst.Type.Equals(span)));

    public int GetLineOffset(int index)
        => LineOffsets[index];

    public byte* GetLineStart(int index)
        => DataStart + GetLineOffset(index);

    public ByteSpan GetLineSpan(int lineIndex)
        => new(GetLineStart(lineIndex), GetLineStart(lineIndex + 1));

    public StepInstance GetInstance(int lineIndex)
        => Instances[lineIndex];

    public StepEntityWithId GetEntityFromLine(int lineIndex)
    {
        var inst = GetInstance(lineIndex);
        if (!inst.IsValid())
            return null;
        var span = GetLineSpan(lineIndex);
        var attr = inst.GetAttributes(span.End());
        var e = new StepEntity(inst.Type, attr);
        var r = new StepEntityWithId(inst.Id, lineIndex, e);
        return r;
    }

    public Dictionary<int, StepEntityWithId> ComputeEntities()
    {
        var r = new Dictionary<int, StepEntityWithId>();
        for (var i = 0; i < GetNumLines(); ++i)
        {
            var e = GetEntityFromLine(i);
            if (e != null)
                r.Add(e.Id, e);
        }
        return r;
    }

    public StepEntityWithId GetEntityFromInst(StepInstance inst, int lineIndex)
    {
        var span = GetLineSpan(lineIndex);
        var attr = inst.GetAttributes(span.End());
        var e = new StepEntity(inst.Type, attr);
        return new StepEntityWithId(inst.Id, lineIndex, e);
    }

    public int GetNumLines()
        => Instances.Length - 1;

    public IEnumerable<StepEntityWithId> GetEntities()
        => Enumerable
            .Range(0, GetNumLines())
            .Select(GetEntityFromLine)
            .WhereNotNull();

    public List<StepEntityWithId> GetEntities(string type)
    {
        type = type.ToUpperInvariant();
        var r = new List<StepEntityWithId>();
        type.WithSpan(span =>
        {
            for (var i = 0; i < GetNumLines(); ++i)
            {
                var inst = GetInstance(i);
                if (inst.IsValid() && inst.Type.Equals(span))
                    r.Add(GetEntityFromInst(inst, i));
            }
        });
        return r;
    }

    public static StepDocument Create(FilePath fp)
        => new StepDocument(fp);
}