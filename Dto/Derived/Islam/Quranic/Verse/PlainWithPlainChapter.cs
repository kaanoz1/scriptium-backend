using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Verse;

public class PlainWithPlainChapter : Plain, ICacheable
{
    public required Chapter.Plain Chapter { get; set; }
}