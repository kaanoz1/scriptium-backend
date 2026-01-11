using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Verse;

public class Both : Complete, ICacheable
{
    public required List<Word.Down> Words { get; set; }
    public required Derived.Islam.Quranic.Chapter.UpToQuran Chapter { get; set; }
}