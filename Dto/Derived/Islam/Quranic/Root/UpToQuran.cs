using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Root;

public class UpToQuran : Complete, ICacheable
{
    public required List<Word.UpToQuran> Words { get; set; }
}