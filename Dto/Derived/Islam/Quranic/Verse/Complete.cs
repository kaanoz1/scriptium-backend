using System.Collections.Generic;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Verse;

public class Complete : Plain
{
    public required List<VerseTranslation.Complete> Translations { get; set; }
    public required List<Dto.Sealed.Transliteration.Plain> Transliterations { get; set; }
}