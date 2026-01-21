using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.VerseTranslation;

public class WithVerse : Complete, ICacheable
{
    public required Verse.TransliterationUpToQuran Verse { get; set; }
}