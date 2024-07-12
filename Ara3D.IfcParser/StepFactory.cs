using System.Diagnostics;

namespace Ara3D.IfcParser;

public static class StepFactory
{
    public static StepValue Create(StepDocument doc, ref int beginToken, int endToken)
    {
        var span = doc.GetTokenSpan(beginToken++);
        Debug.Assert(span.Length > 0);

        var tt = StepTokenizer.LookupToken(span.First());
        switch (tt)
        {
            case TokenType.String:
                Debug.Assert(span.Length >= 2);
                Debug.Assert(span.First() == '\'');
                Debug.Assert(span.Last() == '\'');
                return new StepString(span.Trim(1, 1));

            case TokenType.Symbol:
                Debug.Assert(span.Length >= 2);
                Debug.Assert(span.First() == '.');
                Debug.Assert(span.Last() == '.');
                return new StepSymbol(span.Trim(1, 1));

            case TokenType.Id:
                Debug.Assert(span.Length >= 2);
                Debug.Assert(span.First() == '#');
                return new StepId(span.Skip(1));

            case TokenType.Redeclared:
                Debug.Assert(span.Length == 1);
                Debug.Assert(span.First() == '*');
                return new StepRedeclared();

            case TokenType.Unassigned:
                Debug.Assert(span.Length == 1);
                Debug.Assert(span.First() == '$');
                return new StepUnassigned();

            case TokenType.Number:
                return new StepNumber(span);

            case TokenType.Ident:
                if (doc.GetTokenType(beginToken++) != TokenType.BeginGroup)
                    throw new Exception("Expected an attribute group");
                var attr = CreateAggregate(doc, ref beginToken, endToken);
                return new StepInstance(span, attr);

            case TokenType.BeginGroup:
                return CreateAggregate(doc, ref beginToken, endToken);

            case TokenType.None:
            case TokenType.Whitespace:
            case TokenType.Comment:
            case TokenType.Unknown:
            case TokenType.LineBreak:
            case TokenType.EndOfLine:
            case TokenType.Definition:
            case TokenType.Separator:
            case TokenType.EndGroup:
            default:
                throw new Exception($"Cannot convert token type {tt} to a StepValue");
        }
    }

    public static StepAggregate CreateAggregate(StepDocument doc, ref int beginToken, int endToken)
    {
        var values = new List<StepValue>();
        var tt = doc.GetTokenType(beginToken);
        while (beginToken < endToken && tt != TokenType.EndGroup)
        {
            if (tt == TokenType.Comment 
                || tt == TokenType.Whitespace 
                || tt == TokenType.Separator 
                || tt == TokenType.None)
            {
                // Advance past comments, whitespace, and commas 
                tt = doc.GetTokenType(++beginToken);
            }
            else
            {
                var prevToken = beginToken;
                // This should always increment the "beginToken". Checked in debug builds 
                var curValue = Create(doc, ref beginToken, endToken);
                values.Add(curValue);
                Debug.Assert(beginToken > prevToken);
                tt = doc.GetTokenType(beginToken);
            }
        }

        if (tt != TokenType.EndGroup)
            throw new Exception("Did not reach end of aggregate");

        return new StepAggregate(values);
    }

   
}