namespace Ara3D.IfcParser.Test;

public class StepValue
{
    public readonly StepDocument Source;
    public readonly int Index;

    public StepValue(StepDocument source, int index)
    {
        Source = source;
        Index = index;
    }
}