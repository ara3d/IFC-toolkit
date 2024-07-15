using System.Diagnostics;
using Ara3D.Spans;

namespace Ara3D.IfcParser;

public static unsafe class StepFactory
{
    public static StepAggregate? GetAttributes(this StepInstance inst, byte* lineEnd)
    {
        if (!inst.IsValid())
            return default;

        var ptr = inst.Type.End();
        if (ptr >= lineEnd)
            return default;

        if (*ptr++ != (byte)'(')
        {
            // NOTE: Maybe there is whitespace between the identifier and the parenthesis 
            // This should be exceedingly rare. 
            Debug.Fail("Expected an open parenthesis");
            return default;
        }

        return CreateAggregate(ref ptr, lineEnd);
    }

    public static StepValue Create(ref byte* cur, byte* end)
    {
        var begin = cur;
        var type = StepTokenizer.ParseToken(ref cur, end);
        var span = new ByteSpan(begin, cur);

        switch (type)
        {
            case StepTokenType.String:
                Debug.Assert(span.Length >= 2);
                Debug.Assert(span.First() == '\'');
                Debug.Assert(span.Last() == '\'');
                return new StepString(span.Trim(1, 1));

            case StepTokenType.Symbol:
                Debug.Assert(span.Length >= 2);
                Debug.Assert(span.First() == '.');
                Debug.Assert(span.Last() == '.');
                return new StepSymbol(span.Trim(1, 1));

            case StepTokenType.Id:
                Debug.Assert(span.Length >= 2);
                Debug.Assert(span.First() == '#');
                return new StepId(span.Skip(1));

            case StepTokenType.Redeclared:
                Debug.Assert(span.Length == 1);
                Debug.Assert(span.First() == '*');
                return new StepRedeclared();

            case StepTokenType.Unassigned:
                Debug.Assert(span.Length == 1);
                Debug.Assert(span.First() == '$');
                return new StepUnassigned();

            case StepTokenType.Number:
                return new StepNumber(span);

            case StepTokenType.Ident:
                Debug.Assert(*cur == '(');
                cur++;
                var attr = CreateAggregate(ref cur, end);
                return new StepEntity(span, attr);

            case StepTokenType.BeginGroup:
                return CreateAggregate(ref cur, end);

            case StepTokenType.None:
            case StepTokenType.Whitespace:
            case StepTokenType.Comment:
            case StepTokenType.Unknown:
            case StepTokenType.LineBreak:
            case StepTokenType.EndOfLine:
            case StepTokenType.Definition:
            case StepTokenType.Separator:
            case StepTokenType.EndGroup:
            default:
                throw new Exception($"Cannot convert token type {type} to a StepValue");
        }
    }

    public static StepAggregate CreateAggregate(ref byte* cur, byte* end)
    {
        var begin = cur;
        var values = new List<StepValue>();
        var tt = StepTokenizer.LookupToken(*cur);
        while (cur < end && tt != StepTokenType.EndGroup)
        {
            // Advance past comments, whitespace, and commas 
            if (tt == StepTokenType.Comment 
                || tt == StepTokenType.Whitespace 
                || tt == StepTokenType.Separator 
                || tt == StepTokenType.None)
            {
                tt = StepTokenizer.LookupToken(*cur++);
                continue;
            }
            
            Debug.Assert(tt != StepTokenType.Unknown);

            // Get the next value
            var tmp = cur;
            var curValue = Create(ref cur, end);
            Debug.Assert(cur > tmp);
            values.Add(curValue);
            tt = StepTokenizer.LookupToken(*cur);
        }

        if (tt != StepTokenType.EndGroup)
            throw new Exception("Did not reach end of aggregate");
        cur++;
        return new StepAggregate(values);
    }
}