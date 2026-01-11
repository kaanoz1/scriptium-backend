using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Chapter;

public class Complete : Plain, IHasPlainMeaning, ICacheable
{
    public required List<Sealed.Meaning.Plain> Meanings { get; set; }
}