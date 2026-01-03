using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Islam.Quranic.Chapter;

public class Complete : Plain, IDtoHasMeaning, ICacheable
{
    public required List<ScriptiumBackend.Dto.Shared.Meaning.Plain> Meanings { get; set; }
}