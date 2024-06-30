namespace Ara3D.IfcParser.Test;

public class StepRawData
{
    public readonly byte[] Data;
    public readonly int Start;
    public readonly int End;
}

public class StepDefinition
{
    public readonly int Index;
}


public static class StepValueParser
{
    public static (StepString, int) ParseString(string input)
    {
        if (input.Length < 3 || input[0] != '\'')
            throw new Exception("Not a string");
        var i = 1;
        while (i < input.Length && input[i] != '\'')
        {
            i++;
        }

        if (i == input.Length || input[i] != '\'')
            throw new Exception("Not a string");
        var tmp = input.Substring(1, i - 1);
        return (new StepString(tmp), i + 1);
    }

    public static (StepAggregate, int) ParseAggregate(string input)
    {
        if (input.Length < 3 || input[0] != '(')
            throw new Exception("Not an aggregate");
        var i = 1;
        var list = new List<StepData>();
        while (i < input.Length && input[i] != ')')
        {
            var (local, j) = Parse(input.Substring(i));
            if (j <= 0)
                throw new Exception("Parser has gotten stuck");
            list.Add(local);
            i += j;
            if (i >= input.Length || (input[i] != ')' && input[i] != ','))
                throw new Exception("Expected , or ) character");
            if (input[i] == ',')
                i++;
        }
        return (new StepAggregate(list), i + 1);
    }

    public static (StepSymbol, int) ParseSymbol(string input)
    {
        if (input.Length < 3 || input[0] != '.')
            throw new Exception("Not a symbol");
        var i = 1;
        while (i < input.Length && input[i] != '.')
        {
            if (!char.IsLetter(input[i]) && !char.IsDigit(input[i]) && input[i] != '_' && input[i] != '-')
                throw new Exception("Expected symbol to only contain identifier characters");
            i++;
        }

        if (i == input.Length || input[i] != '.')
            throw new Exception("Not a symbol");
        var tmp = input.Substring(1, i - 2);
        return (new StepSymbol(tmp), i + 1);
    }

    public static (StepNumber, int) ParseNumber(string input)
    {
        var i = 0;
        while (i < input.Length)
        {
            if (!char.IsDigit(input[i]) && input[i] != '.' && input[i] != 'E' && input[i] != 'e' && input[i] != '-' && input[i] != '+')
                break;
            i++;
        }

        if (i == 0)
            throw new Exception("Not a number");
        var tmp = input.Substring(0, i);
        var v = double.Parse(tmp);
        return (new StepNumber(v), i);
    }

    public static (StepReference, int) ParseReference(string input)
    {
        if (input.Length < 1 || input[0] != '#')
            throw new Exception("Not a reference");
        var i = 1;
        while (i < input.Length)
        {
            if (!char.IsDigit(input[i]))
                break;
            i++;
        }
        var tmp = input.Substring(1, i - 1);
        var v = int.Parse(tmp);
        return (new StepReference(v), i);
    }

    public static (StepFunction, int) ParseFunction(string input)
    {
        if (input.Length < 3)
            throw new Exception("Not a function");
        var i = 1;
        while (i < input.Length)
        {
            if (!char.IsLetter(input[i]))
                break;
            i++;
        }
        var name = input.Substring(0, i);
        if (i >= input.Length || input[i] != '(')
            throw new Exception("Not a function");
        var rest = input.Substring(i);
        var (values, j) = ParseAggregate(rest);
        if (j <= 0)
            throw new Exception("Parser has gotten stuck");
        return (new StepFunction(name, values), i + j);
    }

    public static (StepData, int) Parse(string input)
    {
        if (string.IsNullOrEmpty(input))
            return (StepData.Unassigned, 0);
        
        switch (input[0])
        {
            case ' ':
                throw new NotImplementedException();
            case '\'':
                return ParseString(input);
            case '(':
                return ParseAggregate(input);
            case '.':
                return ParseSymbol(input);
            case '#':
                return ParseReference(input);
            case '$':
                return (StepData.Unassigned, 1);
            case '*':
                return (StepData.Redeclared, 1);
            case '-':
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                return ParseNumber(input);
        }
        if (char.IsLetter(input[0]))
            return ParseFunction(input);
        throw new Exception("Not a recognized value");
    }
}