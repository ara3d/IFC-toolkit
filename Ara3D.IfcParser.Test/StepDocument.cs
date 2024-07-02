using Ara3D.Utils;

namespace Ara3D.IfcParser.Test;

public class StepDocument
{
    public readonly StepToken[] Tokens;
    public readonly byte[] Bytes;

    public StepDocument(byte[] bytes)
    {
        Bytes = bytes;
        Tokens = StepTokenizer.CreateTokens(bytes);
    }

    public static StepDocument Create(FilePath filePath)
        => new StepDocument(filePath.ReadAllBytes());
}