using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Islam.Quranic.Word;

public class Plain : IHasSequence
{
    public required int Sequence { get; set; }

    public required string Text { get; set; }
}