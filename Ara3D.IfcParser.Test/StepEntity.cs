namespace Ara3D.IfcParser.Test;

public struct StepEntity
{
    public int Id;
    public int Index;
    public int BeginToken;
    public int EndToken;

    public StepEntity(int begin, int end)
    {
        BeginToken = begin;
        EndToken = end;
    }
}