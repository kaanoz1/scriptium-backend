using System.Collections.Generic;
using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Word;

public class Transliteration : Plain, IHasTransliteration
{
    public List<Model.Sealed.Transliteration> Transliterations { get; set; }
}