using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Islam.Quranic.Root;

public class UpToVerse : Complete, ICacheable
{
    public required List<Dto.Islam.Quranic.Word.UpToVerse> Words { get; set; }
}