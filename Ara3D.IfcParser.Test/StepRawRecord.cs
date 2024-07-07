using System.Runtime.InteropServices;

namespace Ara3D.IfcParser.Test;

/// <summary>
/// Describes where to find a Step record in the the data file.
/// Intended to be used with a StepDocument. 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 0)]
public struct StepRawRecord
{
    public int Id;
    public int BeginToken;
    public int EndToken;

    public StepRawRecord(int begin, int end)
    {
        BeginToken = begin;
        EndToken = end;
    }
}