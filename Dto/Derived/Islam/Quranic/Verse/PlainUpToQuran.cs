using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Verse;

public class PlainUpToQuran : Plain, ICacheable
{
    public required Derived.Islam.Quranic.Chapter.UpToQuran Chapter { get; set; }
}