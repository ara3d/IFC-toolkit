using Ara3D.Parakeet;

namespace Ara3D.IfcParser.Test;
public enum EntityError
{
    None,
    NoHash,
    NoSemicolon,
    NoEquals,
    NoId,
    NoParen,
}

public class StepEntity
{
    public int Id;
    public int Index;
    public int Length;
    public string Name;
    public string RawData;
    public StepAggregate Data;
    public EntityError Error = EntityError.None;
    public ParserState State;
}   