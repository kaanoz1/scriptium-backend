using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Islam.Quranic.Verse;

public class Down : Complete, ICacheable
{
    public required List<Dto.Islam.Quranic.Word.Down> Word { get; set; }
}