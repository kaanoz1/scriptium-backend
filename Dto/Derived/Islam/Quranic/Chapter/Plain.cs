using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Chapter;

public class Plain: IHasSequence
{
    public required string Name { get; init; }

    public required int Sequence { get; set; }
}