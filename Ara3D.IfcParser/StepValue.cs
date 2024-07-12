using Ara3D.Spans;
using Ara3D.Utils;

namespace Ara3D.IfcParser;

public class StepValue
{
}

public class StepAggregate : StepValue
{
    public readonly List<StepValue> Values;

    public StepAggregate(List<StepValue> values)
        => Values = values;

    public override string ToString()
        => $"({Values.JoinStringsWithComma()})";
}

public class StepString : StepValue
{
    public readonly ByteSpan Value;

    public StepString(ByteSpan value)
        => Value = value;

    public override string ToString()
        => $"'{Value}'";
}

public class StepSymbol : StepValue
{
    public readonly ByteSpan Name;

    public StepSymbol(ByteSpan name)
        => Name = name;

    public override string ToString() 
        => $".{Name}.";
}

public class StepNumber : StepValue
{
    public readonly ByteSpan Span;
    public double Value => Span.ToDouble();

    public StepNumber(ByteSpan span)
        => Span = span;

    public override string ToString() 
        => $"{Value}";
}

public class StepId : StepValue
{
    public readonly ByteSpan Span;
    public int Id => Span.ToInt();

    public StepId(ByteSpan span)
        => Span = span;

    public override string ToString()
        => $"#{Id}";
}

public class StepUnassigned : StepValue
{
    public static StepUnassigned Default 
        = new StepUnassigned();

    public override string ToString()
        => "$";
}

public class StepRedeclared : StepValue
{
    public static StepRedeclared Default 
        = new StepRedeclared();

    public override string ToString()
        => "*";
}

public class StepInstance : StepValue
{
    public ByteSpan Name;
    public StepAggregate Attributes;

    public StepInstance(ByteSpan name, StepAggregate attributes)
    {
        Name = name;
        Attributes = attributes; 
    }

    public override string ToString()
    {
        return $"{Name}{Attributes}";
    }
}
