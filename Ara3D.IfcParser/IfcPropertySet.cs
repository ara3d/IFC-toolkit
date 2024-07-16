using Ara3D.Utils;

namespace Ara3D.IfcParser;

public class IfcPropertySet
{
    public string GlobalId;
    public int OwnerHistoryId;
    public string Name;
    public string Description;
    public List<int> Properties;

    public IfcPropertySet(StepAggregate agg)
    {
        var vals = agg.Values;
        if (vals.Count != 5)
            throw new Exception("Expected 5 values");
        
        GlobalId = vals[0].AsString();
        OwnerHistoryId = vals[1].AsId();
        Name = vals[2].AsString();
        Description = vals[3].AsString();
        Properties = vals[4].AsIdList();
    }

    public override string ToString()
    {
        var propIds = Properties.Select(p => $"#{p}").JoinStringsWithComma();
        return $"IFCPROPERTYSET({GlobalId}, #{OwnerHistoryId}, {Name}, {Description}, ({propIds}))";
    }
}

public class IfcPropertyValue
{
    public string Name;
    public string Description;
    public string Value;
    public string Unit;
    public string ValueEntityType;

    public IfcPropertyValue(StepAggregate agg)
    {
        var vals = agg.Values;
        if (vals.Count != 4)
            throw new Exception("Expected 4 values");

        Name = vals[0].AsString();
        Description = vals[1].AsString();
        var inst = vals[2];
        if (inst is StepEntity se)
        {
            ValueEntityType = se.EntityType.ToString();
            Value = se.Attributes.ToString();
        }
        else
        {
            ValueEntityType = "UNKNOWN";
        }
        Unit = vals[3].ToString();
    }

    public override string ToString()
        => $"IFCPROPERTYSINGLEVALUE('{Name}', {Description}, {Value}, {Unit})";
    
    public string PropertyDescriptor()
        => $"{Name}:{Description}:{Unit}";
}

public class IfcPropertyData
{ 
}
