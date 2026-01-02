using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Interfaces;

public interface IHasTransliteration
{
    public List<Transliteration> Transliterations { get; set; }
}