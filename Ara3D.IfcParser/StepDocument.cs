using Ara3D.Logging;
using Ara3D.Spans;
using Ara3D.Utils;

namespace Ara3D.IfcParser;

public unsafe class StepDocument : IDisposable
{
    public readonly FilePath FilePath;
    public readonly byte* DataStart;
    public readonly SimdMemory Data;
    
    public readonly StepInstance[] Instances;
    public readonly StepInstanceLookup Lookup;
    public readonly List<int> Lines;

    public StepDocument(FilePath filePath, ILogger logger = null)
    {
        FilePath = filePath;
        logger ??= new Logger(LogWriter.ConsoleWriter, "Ara 3D Step Document Loader");

        logger.Log($"Loading {filePath.GetFileSizeAsString()} of data from {filePath.GetFileName()}");
        Data = SimdReader.ReadAllBytes(filePath);
        DataStart = Data.BytePtr;

        logger.Log($"Computing the start of each line");
        // NOTE: this estimates that the average line length is more than 16 characters. 
        // This is a reasonable estimate. Only very degenerate files would not meet that. 
        var cap = Data.NumBytes / 16;
        Lines = new List<int>(cap);
        
        // We are going to report the beginning of the lines, while the "ComputeLines" function
        // will compute the ends of lines.
        var currentLine = 1;
        for (var i=0; i < Data.NumVectors; i++)
        {
            StepTokenizer.ComputeLines(Data.VectorPtr[i], ref currentLine, Lines);
        }
        logger.Log($"Found {Lines.Count} lines");

        logger.Log($"Creating instance records");
        Instances = new StepInstance[Lines.Count];
        var cntValid = 0;

        // NOTE: this could be parallelized
        // NOTE: if a string has a newline in it, then they will need to be pieced together.
        // Possibly I can detect this by looking for an odd number of apostrophes. 
        for (var i = 0; i < Instances.Length - 1; i++)
        {
            var lineStart = Lines[i];
            var lineEnd = Lines[i + 1];
            var inst = StepTokenizer.ParseLine(DataStart, lineStart, lineEnd);
            Instances[i] = inst;
            if (inst.IsValid())
                cntValid++;
        }

        logger.Log($"Found {cntValid} instances");

        logger.Log("Creating instance ID lookup");
        Lookup = new(Instances);
        logger.Log($"Completed creation of STEP document from {filePath.GetFileName()}");
    }

    public void Dispose()
    {
        Data.Dispose();
    }

    public StepInstance[] GetInstances()
    {
        return Instances;
    }

    public IEnumerable<StepInstance> GetInstances(ByteSpan type)
    {
        return Instances.Where(inst => inst.Type.Equals(type));
    }
}