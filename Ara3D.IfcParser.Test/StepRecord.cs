using System.Diagnostics;

namespace Ara3D.IfcParser.Test;

/// <summary>
/// Constructed as needed.
/// This is because StepValue is expensive to create. 
/// </summary>
public readonly struct StepRecord
{
    public readonly int Id;
    public readonly StepInstance Value;
    
    public StepRecord(int id, StepInstance value)
    {
        Id = id;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Id} = {Value}";
    }
}
