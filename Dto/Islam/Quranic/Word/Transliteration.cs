using ScriptiumBackend.Interfaces;
using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Dto.Islam.Quranic.Word;

public class Transliteration : Plain, IHasTransliteration
{
    public List<Model.Shared.Transliteration> Transliterations { get; set; }
}