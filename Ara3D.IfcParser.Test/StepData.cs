using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ara3D.IfcParser.Test;

public class StepData
{
    public static StepUnassigned Unassigned = StepUnassigned.Default;
    public static StepRedeclared Redeclared = StepRedeclared.Default;
}

public class StepNumber : StepData
{
    public readonly double Value;
    public StepNumber(double value)
    {
        Value = value;
    }
    public override string ToString()
    {
        return $"{Value}";
    }
}

public class StepAggregate : StepData
{
    public readonly IReadOnlyList<StepData> Children;
    public StepAggregate(IReadOnlyList<StepData> children)
    {
        Children = children;
    }

    public override string ToString()
    {
        return $"({string.Join(",", Children)})";
    }
}

public class StepSymbol : StepData
{
    public readonly string Name;

    public StepSymbol(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return $".{Name}.";
    }
}

public class StepReference : StepData
{
    public readonly int Id;

    public StepReference(int id)
    {
        Id = id;
    }

    public override string ToString()
    {
        return $"#{Id}";
    }
}

public class StepFunction : StepData
{
    public string Name;
    public StepAggregate Data;

    public StepFunction(string name, StepAggregate data)
    {
        Name = name;
        Data = data;
    }

    public override string ToString()
    {
        return $"{Name}{Data}";
    }

}

public class StepString : StepData
{
    public readonly string Data;

    public StepString(string data)
    {
        Data = data;
    }

    public override string ToString()
    {
        return $"'{Data}'";
    }
}

public class StepUnassigned : StepData
{
    public static StepUnassigned Default = new StepUnassigned();

    public override string ToString()
    {
        return "$";
    }
}

public class StepRedeclared : StepData
{
    public static StepRedeclared Default = new StepRedeclared();

    public override string ToString()
    {
        return "*";
    }
}