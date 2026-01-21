using System.Collections.Generic;
using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Word;

public class Complete : Plain, IHasPlainMeaning, IHasSequence, IHasPlainTransliteration
{
    public required List<Sealed.Meaning.Plain> Meanings { get; set; }
    public required List<Dto.Sealed.Transliteration.Plain> Transliterations { get; set; }
}