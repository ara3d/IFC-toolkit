namespace Ara3D.IfcParser.Test;

public class StepDefinition
{
    public int Id;
    public StepValue Value;

    public StepDefinition(int id, StepValue value)
    {
        Id = id; 
        Value = value; 
    }
}