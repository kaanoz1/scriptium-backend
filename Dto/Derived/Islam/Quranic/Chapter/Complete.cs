using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Chapter;

public class Complete : Plain, IDtoHasMeaning, ICacheable
{
    public required List<Sealed.Meaning.Plain> Meanings { get; set; }
}