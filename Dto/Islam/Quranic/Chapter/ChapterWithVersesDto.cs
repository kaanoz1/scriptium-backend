using ScriptiumBackend.Dto.Islam.Quranic.Verse;
using ScriptiumBackend.Interfaces;
using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Dto.Islam.Quranic.Chapter;

public class ChapterWithVersesDto : IHasMeaning, IHasSequence, ICacheable
{
    public required string Name { get; set; }

    public int Sequence { get; set; }

    public required List<PlainVerseDto> Verses { get; init; }

    public required List<Meaning> Meanings { get; set; }
}