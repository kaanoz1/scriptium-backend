using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Islam.Quranic.Chapter;

public class Plain: IHasSequence
{
    public required string Name { get; init; }

    public int Sequence { get; set; }
}