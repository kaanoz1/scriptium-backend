using ScriptiumBackend.Interfaces;
using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Dto.Islam.Quranic.Word;

public class Complete : Plain, IHasMeaning, IHasSequence, IHasTransliteration
{
    public required List<Model.Shared.Meaning> Meanings { get; set; }
    public required List<Model.Shared.Transliteration> Transliterations { get; set; }
}