using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Word;

public class Complete : Plain, IDtoHasMeaning, IHasSequence, IHasTransliteration
{
    public required List<Sealed.Meaning.Plain> Meanings { get; set; }
    public required List<Model.Sealed.Transliteration> Transliterations { get; set; }
}