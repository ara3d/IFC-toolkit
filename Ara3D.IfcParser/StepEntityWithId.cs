namespace Ara3D.IfcParser;

public class StepEntityWithId
{
    public readonly int Id;
    public readonly int LineIndex;
    public readonly StepEntity Entity;

    public List<StepValue> AttributeValues
        => Entity.Attributes.Values;

    public string EntityType 
        => Entity.EntityType.ToString();

    public StepEntityWithId(int id, int lineIndex, StepEntity entity)
    {
        Id = id;
        LineIndex = lineIndex;
        Entity = entity;
    }

    public bool IsEntityType(string str)
        => EntityType == str;

    public override string ToString()
        => $"{LineIndex}: #{Id}={Entity};";

    public int Count => AttributeValues.Count;

    public StepValue this[int i] 
        => AttributeValues[i];
    
    public bool IsLeaf
        => !AttributeValues.Any(a => a is StepId);
}
