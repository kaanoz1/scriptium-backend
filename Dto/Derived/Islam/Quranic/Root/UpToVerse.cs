using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Root;

public class UpToVerse : Complete, ICacheable
{
    public required List<Word.UpToVerse> Words { get; set; }
}