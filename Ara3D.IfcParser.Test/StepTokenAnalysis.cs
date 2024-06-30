namespace Ara3D.IfcParser.Test;

public class StepTokenAnalysis
{
    public byte[] Bytes;
    public StepToken[] Tokens;
    public TokenType TokenType;
    public Dictionary<string, List<int>> Indices { get; } = new Dictionary<string, List<int>>();

    public StepTokenAnalysis(byte[] bytes, StepToken[] tokens, TokenType tokenType)
    {
        Bytes = bytes;
        Tokens = tokens;
        TokenType = tokenType;
        var i = 0;
        foreach (var token in Tokens)
        {
            if (token.Type != TokenType)
                continue;
            var s = token.GetString(Bytes);
            if (!Indices.ContainsKey(s))
                Indices[s] = new List<int>();
            Indices[s].Add(i++);
        }
        TokenType = tokenType;
    }
}