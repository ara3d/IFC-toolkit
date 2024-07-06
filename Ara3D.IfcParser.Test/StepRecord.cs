using System.Diagnostics;

namespace Ara3D.IfcParser.Test;

/// <summary>
/// Constructed as needed.
/// This is because StepValue is expensive to create. 
/// </summary>
public readonly ref struct StepRecord
{
    public readonly int Id;
    public readonly StepInstance Value;
    
    public StepRecord(int id, StepInstance value)
    {
        Id = id;
        Value = value;
    }
}