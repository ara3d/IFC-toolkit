using System.Diagnostics;
using Ara3D.Buffers;
using Ara3D.Buffers.Modern;
using Ara3D.Utils;

namespace Ara3D.IfcParser;

public class StepValue
{
}

public class StepEntity : StepValue
{
    public readonly ByteSpan EntityType;
    public readonly StepAggregate Attributes;

    public StepEntity(ByteSpan entityType, StepAggregate attributes)
    {
        Debug.Assert(!entityType.IsNull());
        EntityType = entityType;
        Attributes = attributes;
    }

    public override string ToString()
        => $"{EntityType}{Attributes}";
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

    public static StepString Create(StepToken token)
    {
        var span = token.Span;
        Debug.Assert(token.Type == StepTokenType.String);
        Debug.Assert(span.Length >= 2);
        Debug.Assert(span.First() == '\'');
        Debug.Assert(span.Last() == '\'');
        return new StepString(span.Trim(1, 1));
    }

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

    public static StepSymbol Create(StepToken token)
    {
        Debug.Assert(token.Type == StepTokenType.Symbol);
        var span = token.Span;
        Debug.Assert(span.Length >= 2);
        Debug.Assert(span.First() == '.');
        Debug.Assert(span.Last() == '.');
        return new StepSymbol(span.Trim(1, 1));
    }
}

public class StepNumber : StepValue
{
    public readonly ByteSpan Span;
    public double Value => Span.ToDouble();

    public StepNumber(ByteSpan span)
        => Span = span;

    public override string ToString() 
        => $"{Value}";

    public static StepNumber Create(StepToken token)
    {
        Debug.Assert(token.Type == StepTokenType.Number);
        var span = token.Span;
        return new(span);
    }
}

public class StepId : StepValue
{
    public readonly ByteSpan Span;
    public int Id => Span.ToInt();

    public StepId(ByteSpan span)
        => Span = span;

    public override string ToString()
        => $"#{Id}";

    public static StepId Create(StepToken token)
    {
        Debug.Assert(token.Type == StepTokenType.Id);
        var span = token.Span;
        Debug.Assert(span.Length >= 2);
        Debug.Assert(span.First() == '#');
        return new StepId(span.Skip(1));
    }
}

public class StepUnassigned : StepValue
{
    public static StepUnassigned Default 
        = new StepUnassigned();

    public override string ToString()
        => "$";

    public static StepUnassigned Create(StepToken token)
    {
        Debug.Assert(token.Type == StepTokenType.Unassigned);
        var span = token.Span;
        Debug.Assert(span.Length == 1);
        Debug.Assert(span.First() == '$');
        return Default;
    }
}

public class StepRedeclared : StepValue
{
    public static StepRedeclared Default 
        = new StepRedeclared();

    public override string ToString()
        => "*";

    public static StepRedeclared Create(StepToken token)
    {
        Debug.Assert(token.Type == StepTokenType.Redeclared);
        var span = token.Span;
        Debug.Assert(span.Length == 1);
        Debug.Assert(span.First() == '*');
        return Default;
    }
}

public static class StepValueExtensions
{
    public static int AsId(this StepValue value)
        => value is StepUnassigned 
            ? 0 
            : ((StepId)value).Id;

    public static string AsString(this StepValue value)
        => value is StepUnassigned 
            ? "" 
            : ((StepString)value).Value.ToString();

    public static double AsNumber(this StepValue value)
        => value is StepUnassigned 
            ? 0 
            : ((StepNumber)value).Value;

    public static List<StepValue> AsList(this StepValue value)
        => value is StepUnassigned
            ? new List<StepValue>()
            : ((StepAggregate)value).Values;

    public static List<int> AsIdList(this StepValue value)
        => value is StepUnassigned
            ? new List<int>()
            : value.AsList().Select(AsId).ToList();
}
