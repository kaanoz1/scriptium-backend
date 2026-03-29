using System.Collections.Generic;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.VerseTranslation;

public class Complete : Plain
{
    public required List<Sealed.Footnote.Plain> Footnotes { get; init; } = [];
    
    public required Translation.Complete Translation { get; init; }
    
}