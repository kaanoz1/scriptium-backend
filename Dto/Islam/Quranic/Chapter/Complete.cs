using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Islam.Quranic.Chapter;

public class Complete : Plain, IDtoHasMeaning
{
    public required List<ScriptiumBackend.Dto.Shared.Meaning.Plain> Meanings { get; set; }
}