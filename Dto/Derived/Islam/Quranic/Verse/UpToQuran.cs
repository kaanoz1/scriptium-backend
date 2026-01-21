using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Verse;

public class UpToQuran : Complete, ICacheable
{
    public required Derived.Islam.Quranic.Chapter.UpToQuran Chapter { get; set; }
    
}