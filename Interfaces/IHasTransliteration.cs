using ScriptiumBackend.Model.Sealed;

namespace ScriptiumBackend.Interfaces;

public interface IHasTransliteration
{
    public List<Transliteration> Transliterations { get; set; }
}