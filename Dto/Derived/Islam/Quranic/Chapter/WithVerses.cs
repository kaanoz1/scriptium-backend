using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Chapter;


public class WithVerses : Complete
{
    public required List<Verse.Complete> Verses { get; init; }

}