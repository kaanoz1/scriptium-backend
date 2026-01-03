using ScriptiumBackend.Interfaces;
using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Dto.Islam.Quranic.Word;

public class Complete : Plain, IDtoHasMeaning, IHasSequence, IHasTransliteration
{
    public required List<Dto.Shared.Meaning.Plain> Meanings { get; set; }
    public required List<Model.Shared.Transliteration> Transliterations { get; set; }
}