using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Translation;

public class Complete : Plain, ICacheable
{
    public required List<Dto.Sealed.Author.Complete> Authors { get; init; }
}